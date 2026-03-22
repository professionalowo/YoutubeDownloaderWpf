using Microsoft.Extensions.DependencyInjection;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Setup;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddDownloadServices<TYoutubeDownloader>(IDirectory root)
            where TYoutubeDownloader : YoutubeDownloaderBase
        {
            serviceCollection.AddSingleton<TYoutubeDownloader>();
            serviceCollection.AddSingleton<IDirectory>(_ =>
            {
                var dir = root.ChildDirectory("Downloads");
                dir.Init();
                return dir;
            });
            serviceCollection.AddTransient<YoutubeHttpHandler>();
            serviceCollection.AddHttpClient<YoutubeClient>()
                .UseDefaultHttpConfig()
                .AddHttpMessageHandler<YoutubeHttpHandler>();
            serviceCollection.AddTransient<YoutubeClient>(s =>
            {
                var httpClient = s.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(YoutubeClient));
                return new YoutubeClient(httpClient);
            });
            serviceCollection.AddTransient<DownloadFactory>();
            serviceCollection.AddHttpClient<VideoDownloadService>()
                .UseDefaultHttpConfig()
                .AddHttpMessageHandler<YoutubeHttpHandler>();
            serviceCollection.AddSingleton<ConverterFactory>();
            serviceCollection.AddSingleton(_ =>
            {
                var ffmpegFolder = root.ChildDirectory("ffmpeg");
                var ffmpegConfig = new FfmpegConfig(ffmpegFolder, FfmpegConfig.SourceUri);
                var ffmpegFactory = new FfmpegConfigFactory(ffmpegConfig);
                return ffmpegFactory.ResolveConfig();
            });
            serviceCollection.AddHttpClient<FfmpegDownloader>()
                .UseDefaultHttpConfig();

            return serviceCollection;
        }
    }
}