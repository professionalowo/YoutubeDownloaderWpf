using Microsoft.Maui;
using Microsoft.Maui.Controls;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Maui;

public partial class App : Application
{
    private readonly FfmpegDownloader ffmpegDownloader;
    public App(FfmpegDownloader ffmpegDownloader)
    {
        this.ffmpegDownloader = ffmpegDownloader;
        InitializeComponent();
    }

    protected async override void OnStart()
    {
        base.OnStart();
        if (!ffmpegDownloader.DoesFfmpegExist() && !PlatformUtil.IsMacOS()) //can't do shit on mac
        {
            await ffmpegDownloader.DownloadFfmpeg();
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}