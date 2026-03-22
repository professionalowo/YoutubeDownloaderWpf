using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Services;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Maui.Services.Mp3Player;
using YoutubeDownloader.Maui.Util;
using YoutubeExplode;


namespace YoutubeDownloader.Maui;

public static class MauiProgram
{
    private static readonly Lazy<IDirectory> BaseDirectory = new(() =>
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YoutubeDownloader");
        return new AbsoluteDirectory(path);
    });

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options => { options.SetShouldEnableSnackbarOnWindows(true); })
            .UseMauiCommunityToolkitMediaElement(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services
            .AddDownloadServices(BaseDirectory.Value)
            .AddTransient<Services.YoutubeDownloader>()
            .AddScoped<Mp3Player>();
        return builder.Build();
    }
}