using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Data;
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
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options => { options.SetShouldEnableSnackbarOnWindows(true); })
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services
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

    private static IDirectory CreateDownloadDirectory(IServiceProvider _)
    {
        IDirectory child = new ChildDirectory(BaseDirectory.Value, "Downloads");
        child.Init();
        return child;
    }

    extension(IHttpClientBuilder builder)
    {
        private IHttpClientBuilder UseDefaultHttpConfig(
        )
        {
            return builder
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new SocketsHttpHandler
                    {
                        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                        ConnectTimeout = TimeSpan.FromSeconds(10),
                    })
                .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(30));
        }
    }

    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddDownloadServices()
        {
            serviceCollection.AddTransient<YoutubeHttpHandler>();
            serviceCollection.AddHttpClient<YoutubeClient>()
                .UseDefaultHttpConfig()
                .AddHttpMessageHandler<YoutubeHttpHandler>();
            serviceCollection.AddTransient<YoutubeClient>(s =>
            {
                var httpClient = s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(YoutubeClient));
                return new YoutubeClient(httpClient);
            });
            serviceCollection.AddFfmpeg(new ChildDirectory(BaseDirectory.Value, "ffmpeg"))
                .AddTransient<Services.YoutubeDownloader>()
                .AddSingleton(CreateDownloadDirectory)
                .AddTransient<DownloadFactory<DownloadContext>>()
                .AddSingleton<ConverterFactory<DownloadContext>>();
            return serviceCollection;
        }


        public IServiceCollection AddUpdaters()
        {
            serviceCollection.AddHttpClient<FfmpegDownloader>()
                .UseDefaultHttpConfig();
            return serviceCollection;
        }
    }
}