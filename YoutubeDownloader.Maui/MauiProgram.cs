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
            .UseMauiCommunityToolkit()
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
            .AddUpdaters();
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
    {
        serviceCollection.AddTransient<YoutubeClient>();
        serviceCollection.AddFfmpeg(new ChildDirectory(BaseDirectory.Value, "ffmpeg"));
        serviceCollection.AddTransient<Services.YoutubeDownloader>();
        serviceCollection.AddSingleton<IDirectory>(_ =>
        {
            IDirectory child = new ChildDirectory(BaseDirectory.Value, "Downloads");
            child.Init();
            return child;
        });
        serviceCollection.AddTransient<DownloadFactory<DownloadContext>>();
        serviceCollection.AddSingleton<ConverterFactory<DownloadContext>>();
        return serviceCollection;
    }

    public static IServiceCollection AddUpdaters(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<FfmpegDownloader>();
}