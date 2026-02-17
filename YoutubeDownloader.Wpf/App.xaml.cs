using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Velopack;
using Velopack.Sources;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.AutoUpdater.GitHub;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services.Logging;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Wpf.Controls;
using YoutubeDownloader.Wpf.Services;
using YoutubeDownloader.Wpf.Util;
using YoutubeDownloader.Wpf.Services.Downloader;
using YoutubeExplode;

namespace YoutubeDownloader.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainWindow? _mainWindow;
    private readonly ServiceProvider services;

    public App()
    {
        services = InitializeServices();
        this.DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = services.GetService<ILogger<App>>();
        logger?.LogError("{Error}", e.Exception);
    }

    private static ServiceProvider InitializeServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDownloadServices();
        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddLogging(builder =>
            builder.AddProvider(new FileLoggerProvider("logs.txt"))
                .SetMinimumLevel(LogLevel.Warning)
        );
        serviceCollection.AddUpdaters();
        return serviceCollection.BuildServiceProvider();
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var updater = services.GetService<PackUpdater>()!;
        await updater.CheckForAppUpdates()
            .ConfigureAwait(false);
        var ffmpeg = services.GetService<FfmpegDownloader>()!;
        if (!ffmpeg.DoesFfmpegExist())
        {
            var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Downlaod Ffmpeg",
                MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.OK)
            {
                await ffmpeg.DownloadFfmpeg(IProgress<double>.Null)
                    .ConfigureAwait(false);
            }
        }

        _mainWindow = services.GetService<MainWindow>();
        _mainWindow?.Show();
    }
}

static class ServiceCollectionExtensions
{
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
            serviceCollection.AddTransient<Services.Downloader.YoutubeDownloader>();
            serviceCollection.AddSingleton<IDirectory>(_ =>
            {
                IDirectory dir = new CwdDirectory("Downloads");
                dir.Init();
                return dir;
            });
            serviceCollection.AddTransient<YoutubeHttpHandler>();
            serviceCollection.AddHttpClient<YoutubeClient>()
                .UseDefaultHttpConfig()
                .AddHttpMessageHandler<YoutubeHttpHandler>();
            serviceCollection.AddTransient<YoutubeClient>(s =>
            {
                var httpClient = s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(YoutubeClient));
                return new YoutubeClient(httpClient);
            });
            serviceCollection.AddTransient<DownloadFactory<DownloadStatusContext>>();
            serviceCollection.AddSingleton<ConverterFactory>();
            return serviceCollection;
        }

        public IServiceCollection AddUpdaters()
        {
            serviceCollection.AddSingleton(new FfmpegConfigFactory(FfmpegDownloader.Config.Default)
                .ResolveConfig);
            serviceCollection.AddHttpClient<FfmpegDownloader>()
                .UseDefaultHttpConfig();
            serviceCollection.AddTransient<PackUpdater>();
            return serviceCollection;
        }
    }
}