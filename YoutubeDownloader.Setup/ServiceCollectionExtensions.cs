using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Setup.Logging;
using YoutubeDownloader.Setup.Settings;
using YoutubeExplode;

namespace YoutubeDownloader.Setup;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddAppServices<TYoutubeDownloader>(IRootDirectory root)
            where TYoutubeDownloader : YoutubeDownloaderBase
        {
            serviceCollection.AddSingleton<TYoutubeDownloader>();
            serviceCollection.AddSingleton<IDownloadDirectoryFactory, DownloadDirectoryFactory>();
            serviceCollection.AddScoped<IDirectory>(p => p.GetRequiredService<IDownloadDirectoryFactory>().Create());
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
            serviceCollection.AddFfmpeg(root);
            serviceCollection.AddHttpClient<FfmpegDownloader>()
                .UseDefaultHttpConfig();
            serviceCollection.AddLogging(builder =>
                builder.AddProvider(new FileLoggerProvider(root.ChildDirectory("logs").ChildFileName("logs.txt"))));

            return serviceCollection;
        }

        public IServiceCollection AddConfig(IRootDirectory root)
        {
            serviceCollection.AddSingleton(root);
            serviceCollection.AddSingleton<ISettingsService, SettingsService>();
            var config = new ConfigurationBuilder()
                .SetBasePath(root.FullPath)
                .AddJsonFile(SettingsService.settingsFileName, true, true)
                .Build();

            return serviceCollection.Configure<AppConfiguration>(config);
        }

        private IServiceCollection AddFfmpeg(IRootDirectory root)
        {
            var ffmpegFolder = root.ChildDirectory("ffmpeg");
            var ffmpegConfig = new FfmpegConfig(ffmpegFolder, FfmpegConfig.SourceUri);
            var ffmpegFactory = new FfmpegConfigFactory(ffmpegConfig);
            var config = ffmpegFactory.ResolveConfig();
            return serviceCollection.AddSingleton(config);
        }
    }
}