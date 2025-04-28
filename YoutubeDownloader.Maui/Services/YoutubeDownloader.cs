using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Maui.Services;

public class YoutubeDownloader
    : YoutubeDownloaderBase<DownloadContext>
{
    public YoutubeDownloader(
        ConverterFactory converterFactory,
        SystemInfo info,
        ILogger<YoutubeDownloaderBase<DownloadContext>> logger,
        DownloadFactory<DownloadContext> downloadFactory,
        IDirectory downloads) : base(converterFactory, info, logger, downloadFactory, downloads)
    {
        DownloadFinished += OnDownloadFinished;
    }

    private static void OnDownloadFinished(object? sender, EventArgs e)
    {
        var toast = Toast.Make("Download finished", ToastDuration.Long);
        toast.Show().GetAwaiter().GetResult();
    }

    protected override DownloadContext ContextFactory(string name, double size)
        => new(name, size);

    protected override Task DispatchToUi(Action action, CancellationToken token = default)
        => MainThread.InvokeOnMainThreadAsync(action);
}