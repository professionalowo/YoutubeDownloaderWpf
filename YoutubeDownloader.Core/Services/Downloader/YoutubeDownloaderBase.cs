using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;


namespace YoutubeDownloader.Core.Services.Downloader;

public abstract class YoutubeDownloaderBase<TContext>(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloaderBase<TContext>> logger,
    DownloadFactory<TContext> downloadFactory,
    IDirectory downloads) : IDownloader<TContext>, INotifyPropertyChanged where TContext : IConverter.IConverterContext
{
    private readonly Lock _cancellationSourceLock = new();
    private readonly Lock _statusesLock = new();


    [StringSyntax(StringSyntaxAttribute.Uri)]
    public string Url
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "";

    public bool ForceMp3
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = true;

    public ObservableCollection<TContext> DownloadStatuses
    {
        get
        {
            lock (_statusesLock)
            {
                return field;
            }
        }
        set
        {
            lock (_statusesLock)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = [];

    private CancellationTokenSource CancellationSource
    {
        get
        {
            lock (_cancellationSourceLock)
            {
                return field;
            }
        }
        set
        {
            lock (_cancellationSourceLock)
            {
                field = value;
            }
        }
    } = new();


    protected abstract Task DispatchToUi(Action action, CancellationToken token = default);

    public async Task Download()
    {
        await DispatchToUi(ClearStatuses)
            .ConfigureAwait(false);
        await DownloadAction(Url, CancellationSource.Token)
            .ConfigureAwait(false);
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

    private async Task ProcessChannel(ChannelReader<VideoDownload<TContext>> reader, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(reader.ReadAllAsync(token), token, ProcessDownload);
    }

    private async ValueTask ProcessDownload(VideoDownload<TContext> download, CancellationToken token = default)
    {
        var converter = converterFactory.GetConverter(ForceMp3);
        var ((stream, segments), context) = await download.GetStreamAsync(ContextFactory, token)
            .ConfigureAwait(false);
        var fileName = downloads.ChildFileName(segments);
        await DispatchToUi(() => DownloadStatuses.Add(context), token)
            .ConfigureAwait(false);
        await using var mediaStream = stream;
        await converter.Convert(mediaStream, fileName, context, token)
            .ConfigureAwait(false);
    }

    private async Task DownloadAction([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default)
    {
        var enumerable = downloadFactory.Get(url, token)
            .ConfigureAwait(false);
        var channel = Channel.CreateBounded<VideoDownload<TContext>>(info.Cores);
        var consumer = ProcessChannel(channel.Reader, token)
            .ConfigureAwait(false);
        try
        {
            await foreach (var download in enumerable)
            {
                await channel.Writer.WriteAsync(download, token)
                    .ConfigureAwait(false);
            }

            channel.Writer.Complete();
            await consumer;
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex.InnerException is OperationCanceledException)
        {
            //ignore these since they are most likely produced by the user
        }
        catch (Exception e)
        {
            logger.LogError("{Error}", e);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}