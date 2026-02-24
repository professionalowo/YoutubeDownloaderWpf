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
    ConverterFactory converterFactory,
    ILogger<YoutubeDownloaderBase<TContext>> logger,
    DownloadFactory<TContext> downloadFactory,
    IDirectory downloads)
    : IDownloader<TContext>, INotifyPropertyChanged where TContext : IConverter<TContext>.IConverterContext
{
    protected IDirectory Downloads => downloads;

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

    public bool IsPrefetching
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = false;

    public bool IsDownloading
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = false;

    public ObservableCollection<TContext> DownloadStatuses { get; } = [];

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

    private IConverter<TContext> Converter => converterFactory.GetConverter<TContext>(ForceMp3);

    protected abstract Task DispatchToUi(Action action, CancellationToken token = default);

    public async Task Download()
    {
        await DispatchToUi(ClearStatuses)
            .ConfigureAwait(false);
        IsDownloading = true;
        IsPrefetching = true;
        try
        {
            await DownloadAction(Url, CancellationSource.Token)
                .ConfigureAwait(false);
            OnDownloadSuccess(Url);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex.InnerException is OperationCanceledException)
        {
            //ignore these since they are most likely produced by the user
        }
        catch (Exception e)
        {
            logger.LogError("{Error}", e);
            OnDownloadFailed(Url, e);
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

    private Task ReadAllDownloads(ChannelReader<VideoDownload<TContext>> reader, CancellationToken token = default)
        => Parallel.ForEachAsync(
            reader.ReadAllAsync(token),
            token,
            ProcessDownloadFactory(Converter)
        );

    private Task WriteAllDownloads(ChannelWriter<VideoDownload<TContext>> writer, string url,
        CancellationToken token = default)
        => Parallel.ForEachAsync(GetDownloadsAsync(url, token), token, writer.WriteAsync);

    private Func<VideoDownload<TContext>, CancellationToken, ValueTask> ProcessDownloadFactory(
        IConverter<TContext> converter)
        => (download, token) => ProcessDownload(converter, download, token);

    private async ValueTask ProcessDownload(IConverter<TContext> converter, VideoDownload<TContext> download,
        CancellationToken token = default)
    {
        var ((stream, segments), context) = await download.GetStreamAsync(ContextFactory, token)
            .ConfigureAwait(false);
        var uiTask = AddDownloadStatus(context, token);
        var fileName = downloads.ChildFileName(segments);
        await using var mediaStream = stream;
        await converter.Convert(mediaStream, fileName, context, token)
            .ConfigureAwait(false);
        await uiTask.ConfigureAwait(false);
    }

    private async Task DownloadAction([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default)
    {
        var (rx, tx) = Channel.CreateBounded<VideoDownload<TContext>>(new BoundedChannelOptions(SystemInfo.Cores)
        {
            SingleWriter = false,
            SingleReader = false,
            AllowSynchronousContinuations = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        var consumer = ReadAllDownloads(rx, token);
        await WriteAllDownloads(tx, url, token).ConfigureAwait(false);
        tx.Complete();
        await consumer.ConfigureAwait(false);
    }

    private Task AddDownloadStatus(TContext context, CancellationToken token = default) =>
        DispatchToUi(() =>
        {
            IsPrefetching = false;
            DownloadStatuses.Add(context);
        }, token);

    protected event EventHandler<DownloadSuccessEventArgs>? DownloadSuccess;
    protected event EventHandler<DownloadFailedEventArgs>? DownloadFailed;

    private void OnDownloadFinished()
    {
        IsPrefetching = false;
        IsDownloading = false;
    }

    private void OnDownloadSuccess(string url)
    {
        OnDownloadFinished();
        DownloadSuccess?.Invoke(this, new DownloadSuccessEventArgs(url));
    }

    private void OnDownloadFailed(string url, Exception error)
    {
        OnDownloadFinished();
        DownloadFailed?.Invoke(this, new DownloadFailedEventArgs(url, error));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    protected record DownloadSuccessEventArgs(string Url);

    protected record DownloadFailedEventArgs(string Url, Exception Error);
}