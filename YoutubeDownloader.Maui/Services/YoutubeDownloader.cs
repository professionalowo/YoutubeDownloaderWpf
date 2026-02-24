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

public sealed partial class YoutubeDownloader
    : YoutubeDownloaderBase<DownloadContext>
{
    public YoutubeDownloader(
        ConverterFactory converterFactory,
        ILogger<YoutubeDownloaderBase<DownloadContext>> logger,
        DownloadFactory<DownloadContext> downloadFactory,
        IDirectory downloads) : base(converterFactory, logger, downloadFactory, downloads)
        => DownloadFinished += OnDownloadFinished;


    private void OnDownloadFinished(object? sender, DownloadFinishedEventArgs e)
        => DispatchToUi(() => e.Error is not null ? ShowErrorToast(e.Error) : ShowFinishedToast())
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    private async Task ShowErrorToast(Exception error)
    {
        using var toast = Toast.Make($"Download failed: {error.Message}", ToastDuration.Long);
        await toast.Show(CancellationSource.Token)
            .ConfigureAwait(false);
    }

    private async Task ShowFinishedToast()
    {
        using var toast = Toast.Make("Download finished", ToastDuration.Long);
        await toast.Show(CancellationSource.Token)
            .ConfigureAwait(false);
    }

    protected override DownloadContext ContextFactory(string name, double size)
        => new(name, size);

    protected override Task DispatchToUi(Action action, CancellationToken token = default)
        => MainThread.InvokeOnMainThreadAsync(action);

    private static Task DispatchToUi(Func<Task> action)
        => MainThread.InvokeOnMainThreadAsync(action);
}