using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using YoutubeDownloader.Wpf.Controls;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;

namespace YoutubeDownloader.Wpf.Services.Downloader;

public class Downloader : DownloaderBase
{
    public Downloader(
        YoutubePlatformService youtube,
        ConverterFactory converterFactory,
        ILogger<Downloader> logger)
        : base(youtube, converterFactory, logger)
    {
        DownloadSuccess += OnDownloadSuccess;
        DownloadFailed += OnDownloadFailed;
    }

    private static void OnDownloadSuccess(object? sender, DownloadSuccessEventArgs e)
        => new ToastContentBuilder()
            .AddText("Download Finished")
            .Show();

    private static void OnDownloadFailed(object? sender, DownloadFailedEventArgs e)
        => new ToastContentBuilder()
            .AddText("Download Failed")
            .Show();

    protected override Task DispatchToUi(Action action, CancellationToken token = default)
        => Dispatch(action, token).Task;

    protected override DownloadStatusContext ContextFactory(string name, double size)
        => new(name, size);

    private static DispatcherOperation Dispatch(Action action, CancellationToken token = default) =>
        Application.Current.Dispatcher.InvokeAsync(action, DispatcherPriority.Render, token);
}