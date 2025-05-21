using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;


namespace YoutubeDownloader.Core.Services.Downloader;

public abstract partial class YoutubeDownloaderBase<TContext>(
    ConverterFactory<TContext> converterFactory,
    ILogger<YoutubeDownloaderBase<TContext>> logger,
    DownloadFactory<TContext> downloadFactory,
    IDirectory downloads)
    : IDownloader<TContext>, INotifyPropertyChanged where TContext : IConverter<TContext>.IConverterContext
{
    private readonly Lock _cancellationSourceLock = new();

    [StringSyntax(StringSyntaxAttribute.Uri)]
    public string Url
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public bool ForceMp3
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = true;

    public bool IsFetching
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = false;

    public ObservableCollection<TContext> DownloadStatuses
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = [];

    protected CancellationTokenSource CancellationSource
    {
        get
        {
            lock (_cancellationSourceLock)
            {
                return field;
            }
        }
        private set
        {
            lock (_cancellationSourceLock)
            {
                field = value;
            }
        }
    } = new();

    private IConverter<TContext> Converter => converterFactory.GetConverter(ForceMp3);

    protected abstract Task DispatchToUi(Action action, CancellationToken token = default);

    public async Task Download()
    {
        await DispatchToUi(ClearStatuses)
            .ConfigureAwait(false);
        IsFetching = true;
        try
        {
            await DownloadAction(Url, CancellationSource.Token)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex.InnerException is OperationCanceledException)
        {
            //ignore these since they are most likely produced by the user
        }
        catch (Exception e)
        {
            logger.LogError("{Error}", e);
        }
        finally
        {
            OnDownloadFinished();
        }
    }


    private void ClearStatuses()
    {
        DownloadStatuses.Clear();
        OnPropertyChanged(nameof(DownloadStatuses));
    }

    public async Task Cancel()
    {
        await CancellationSource.CancelAsync()
            .ConfigureAwait(false);
        await DispatchToUi(ClearStatuses)
            .ConfigureAwait(false);
        CancellationSource = new CancellationTokenSource();
    }

    protected abstract TContext ContextFactory(string name, double size);

    private IAsyncEnumerable<VideoDownload<TContext>> GetDownloadsAsync(
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string url, CancellationToken token = default) => downloadFactory.Get(url, token);

    private Task ProcessChannel(ChannelReader<VideoDownload<TContext>> reader, CancellationToken token = default)
        => Parallel.ForEachAsync(
            reader.ReadAllAsync(token),
            token,
            ProcessDownloadFactory(Converter)
        );

    private Func<VideoDownload<TContext>, CancellationToken, ValueTask> ProcessDownloadFactory(
        IConverter<TContext> converter)
        => (download, token) => ProcessDownload(download, converter, token);

    private async ValueTask ProcessDownload(VideoDownload<TContext> download, IConverter<TContext> converter,
        CancellationToken token = default)
    {
        var ((stream, segments), context) = await download.GetStreamAsync(ContextFactory, token)
            .ConfigureAwait(false);
        var uiTask = AddDownloadStatus(context, token);
        var fileName = downloads.ChildFileName(segments);
        await using var mediaStream = stream;
        await converter.Convert(mediaStream, fileName, context, token).ConfigureAwait(false);
        await uiTask.ConfigureAwait(false);
    }

    private async Task DownloadAction([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default)
    {
        var (rx, tx) = Channel.CreateBounded<VideoDownload<TContext>>(SystemInfo.Cores);
        var consumer = ProcessChannel(rx, token);
        await Parallel.ForEachAsync(GetDownloadsAsync(url, token), token, tx.WriteAsync)
            .ConfigureAwait(false);
        tx.Complete();
        await consumer.ConfigureAwait(false);
    }

    private Task AddDownloadStatus(TContext context, CancellationToken token = default) =>
        DispatchToUi(() =>
        {
            IsFetching = false;
            DownloadStatuses.Add(context);
        }, token);

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? DownloadFinished;

    private void OnDownloadFinished()
    {
        DownloadFinished?.Invoke(this, EventArgs.Empty);
        IsFetching = false;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}