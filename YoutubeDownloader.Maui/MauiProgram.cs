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
using YoutubeDownloader.Wpf.Services.Logging;
using YoutubeExplode;


namespace YoutubeDownloader.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddLogging();
        builder.Services.AddHttp();
        builder.Services.AddDownloadServices();
        builder.Services.AddUpdaters();
        return builder.Build();
    }
}


static class ServicesExtensions
{
    private static Lazy<IDirectory> _directory = new (() =>
    {
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Downloads");
        string path = Path.Combine(downloadsPath, "YoutubeDownloader");
        IDirectory dir = new AbsoluteDirectory(path);
        return dir;
    });
    public static IServiceCollection AddHttp(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SocketsHttpHandler>(_ => new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            ConnectTimeout = TimeSpan.FromSeconds(10)
        });
        serviceCollection.AddScoped<HttpClient>();
        return serviceCollection;
    }

    public static IServiceCollection AddDownloadServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<Services.YoutubeDownloader>();
        serviceCollection.AddSingleton<IDirectory>(_ =>
        {
            var dir = _directory.Value;
            dir.Init();
            return dir;
        });
        serviceCollection.AddTransient<YoutubeClient>();
        serviceCollection.AddTransient<DownloadFactory<DownloadContext>>();
        serviceCollection.AddSingleton<ConverterFactory>();
        serviceCollection.AddSingleton<SystemInfo>();
        return serviceCollection;
    }

    public static IServiceCollection AddUpdaters(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUpdater, Updater.Noop>();
        serviceCollection.AddScoped<GitHubVersionClient>();
        serviceCollection.AddSingleton<TaggedVersion>(_ => new(1, 0, 4));
        serviceCollection.AddSingleton<FfmpegDownloader.Config>(new FfmpegConfigFactory(new(_directory.Value)).ResolveConfig);
        serviceCollection.AddSingleton<FfmpegDownloader>();
        return serviceCollection;
    }
}