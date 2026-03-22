using Microsoft.Extensions.DependencyInjection;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace Services;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddDownloadServices(IDirectory root)
        {
            serviceCollection.AddSingleton<IDirectory>(_ =>
            {
                IDirectory dir = new ChildDirectory(root, "Downloads");
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
            serviceCollection.AddSingleton(p =>
            {
                var ffmpegFolder = new ChildDirectory(root, "ffmpeg");
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