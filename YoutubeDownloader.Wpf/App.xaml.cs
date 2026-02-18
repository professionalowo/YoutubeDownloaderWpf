using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
using YoutubeDownloader.Wpf.View;
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
        return serviceCollection.BuildServiceProvider();
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var updater = services.GetService<VelopackService>()!;
        await updater.CheckForAppUpdates();
        var ffmpeg = services.GetService<FfmpegDownloader>()!;
        if (!ffmpeg.DoesFfmpegExist())
        {
            var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Download Ffmpeg",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                var progressWindow = new ProgressDialog();
                progressWindow.Show();

                try
                {
                    await ffmpeg.DownloadFfmpeg(progressWindow.Progress);
                }
                finally
                {
                    progressWindow.Close();
                }
            }
        }

        _mainWindow = services.GetService<MainWindow>();
        _mainWindow?.Show();
    }
}

internal static class ServiceCollectionExtensions
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
            var manager =
                new UpdateManager(new GithubSource(GitHubVersionClient.url, null, false));

            IDirectory root = manager.GetBasePath() is string path
                ? new AbsoluteDirectory(path)
                : new CwdDirectory(".");

            serviceCollection.AddTransient<UpdateManager>(_ => manager);
            serviceCollection.AddTransient<VelopackService>();
            serviceCollection.AddTransient<Services.Downloader.YoutubeDownloader>();
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
                var httpClient = s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(YoutubeClient));
                return new YoutubeClient(httpClient);
            });
            serviceCollection.AddTransient<DownloadFactory<DownloadStatusContext>>();
            serviceCollection.AddSingleton<ConverterFactory>();
            serviceCollection.AddSingleton(
                new FfmpegConfigFactory(new FfmpegConfig(new ChildDirectory(root, "ffmpeg"), FfmpegConfig.SourceUri))
                    .ResolveConfig);
            serviceCollection.AddHttpClient<FfmpegDownloader>()
                .UseDefaultHttpConfig();

            return serviceCollection;
        }
    }
}