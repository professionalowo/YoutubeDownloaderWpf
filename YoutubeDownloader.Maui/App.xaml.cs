using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Maui;

public partial class App : Application
{
    private readonly FfmpegDownloader _ffmpegDownloader;

    public App(FfmpegDownloader ffmpegDownloader)
    {
        _ffmpegDownloader = ffmpegDownloader;
        InitializeComponent();
    }

    protected async override void OnStart()
    {
        base.OnStart();
        if (!_ffmpegDownloader.DoesFfmpegExist() && !PlatformUtil.IsMacOs()) //can't do shit on mac
        {
            await _ffmpegDownloader.DownloadFfmpeg(IProgress<double>.Null)
                .ConfigureAwait(false);
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}