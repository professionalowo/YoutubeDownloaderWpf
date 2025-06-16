using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.AutoUpdater.GitHub;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Maui.Services;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services.Mp3Player;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Maui.Util;
using YoutubeExplode;


namespace YoutubeDownloader.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options => { options.SetShouldEnableSnackbarOnWindows(true); })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddHttp()
            .AddDownloadServices()
            .AddUpdaters()
            .AddScoped<Mp3Player>();
        return builder.Build();
    }
}

internal static class ServicesExtensions
{
    private static readonly Lazy<IDirectory> BaseDirectory = new(() =>
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YoutubeDownloader");
        return new AbsoluteDirectory(path);
    });

    public static IServiceCollection AddHttp(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<SocketsHttpHandler>(_ => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            ConnectTimeout = TimeSpan.FromSeconds(10)
        }).AddScoped<HttpClient>();


    public static IServiceCollection AddDownloadServices(this IServiceCollection serviceCollection)
        => serviceCollection.AddTransient<YoutubeClient>()
            .AddFfmpeg(new ChildDirectory(BaseDirectory.Value, "ffmpeg"))
            .AddTransient<Services.YoutubeDownloader>()
            .AddSingleton(CreateDownloadDirectory)
            .AddTransient<DownloadFactory<DownloadContext>>()
            .AddSingleton<ConverterFactory<DownloadContext>>();

    private static IDirectory CreateDownloadDirectory(IServiceProvider _)
    {
        IDirectory child = new ChildDirectory(BaseDirectory.Value, "Downloads");
        child.Init();
        return child;
    }

    public static IServiceCollection AddUpdaters(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<FfmpegDownloader>();
}