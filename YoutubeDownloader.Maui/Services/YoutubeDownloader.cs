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

public partial class YoutubeDownloader
    : YoutubeDownloaderBase<DownloadContext>
{
    public YoutubeDownloader(
        ConverterFactory<DownloadContext> converterFactory,
        ILogger<YoutubeDownloaderBase<DownloadContext>> logger,
        DownloadFactory<DownloadContext> downloadFactory,
        IDirectory downloads) : base(converterFactory, logger, downloadFactory, downloads)
    {
        DownloadFinished += OnDownloadFinished;
    }

    private static void OnDownloadFinished(object? sender, EventArgs e)
    {
        var toast = Toast.Make("Download finished", ToastDuration.Long);
        DispatchToUi(async () => await toast.Show()).GetAwaiter().GetResult();
    }

    protected override DownloadContext ContextFactory(string name, double size)
        => new(name, size);

    protected override Task DispatchToUi(Action action, CancellationToken token = default)
        => MainThread.InvokeOnMainThreadAsync(action);

    private static Task DispatchToUi(Func<Task> action) =>
        MainThread.InvokeOnMainThreadAsync(action);
}