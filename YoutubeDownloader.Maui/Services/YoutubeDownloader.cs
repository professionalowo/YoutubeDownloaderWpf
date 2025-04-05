using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Maui.Services;

public class YoutubeDownloader(ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloaderBase<DownloadContext>> logger,
    DownloadFactory<DownloadContext> downloadFactory,
    IDirectory downloads) : YoutubeDownloaderBase<DownloadContext>(converterFactory, info, logger, downloadFactory, downloads)
{
    protected override DownloadContext ContextFactory(string name, double size)
    => new(name, size);

    protected override Task DispatchToUi(Action action, CancellationToken token = default)
    => Dispatcher.GetForCurrentThread() switch
        {
            null => Task.Run(action, token),
            var dispatcher => dispatcher.DispatchAsync(action)
        };
}