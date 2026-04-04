using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using YoutubeDownloader.Setup;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Maui.Services.Mp3Player;

namespace YoutubeDownloader.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                    .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YoutubeDownloader");
        var root = new RootDirectory(path);

        builder.Services
            .AddConfig(root)
            .AddDownloadServices<Services.YoutubeDownloader>(root)
            .AddScoped<Mp3Player>()
            .AddTransient<SettingsPage>();
        return builder.Build();
    }
}