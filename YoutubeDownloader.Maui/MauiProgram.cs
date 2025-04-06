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
    private static readonly Lazy<IDirectory> _baseDirectory = new (() =>
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YoutubeDownloader");
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

            IDirectory child = new ChildDirectory(_baseDirectory.Value, "Downloads");
            child.Init();
            return child;
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
        serviceCollection.AddFfmpeg(new ChildDirectory(_baseDirectory.Value, "ffmpeg"));
        serviceCollection.AddSingleton<FfmpegDownloader>();
        return serviceCollection;
    }
}