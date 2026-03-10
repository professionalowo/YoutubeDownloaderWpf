using System.Net;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SoundCloudExplode;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Platform;
using YoutubeDownloader.Core.Services.Downloader.Platform.SoundCloud;
using YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;
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
                        MaxConnectionsPerServer = 10,
                        EnableMultipleHttp2Connections = true,
                        EnableMultipleHttp3Connections = true,

                        AutomaticDecompression = DecompressionMethods.All,
                        InitialHttp2StreamWindowSize = 1024 * 1024,

                        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                        ConnectTimeout = TimeSpan.FromSeconds(10),

                        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                        KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always
                    })
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestVersion = HttpVersion.Version30;
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    client.Timeout = Timeout.InfiniteTimeSpan;
                });
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
                .AddTransient<Services.Downloader>()
                .AddSingleton(CreateDownloadDirectory)
                .AddTransient<YoutubePlatformService>()
                .AddSingleton<ConverterFactory>();

            serviceCollection.AddTransient<SoundCloudClient>();
            serviceCollection.AddHttpClient<SoundCloudPlatformService>()
                .UseDefaultHttpConfig();

            serviceCollection.AddTransient<PlatformServiceDispatcher>();
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