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
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.AutoUpdater.GitHub;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Wpf.Controls;
using YoutubeDownloader.Wpf.Services.Logging;
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
        serviceCollection.AddHttp();
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
        var updater = services.GetService<IUpdater>()!;
        bool isNewVersion = await updater.IsNewVersionAvailable();
        if (isNewVersion)
        {
            await updater.UpdateVersion(KnownFolders.GetDownloadsPath());
        }

        var ffmpeg = services.GetService<FfmpegDownloader>()!;
        if (!ffmpeg.DoesFfmpegExist())
        {
            var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Downlaod Ffmpeg",
                MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.OK)
            {
                await ffmpeg.DownloadFfmpeg();
            }
        }

        _mainWindow = services.GetService<MainWindow>();
        _mainWindow?.Show();
    }
}

static class ServiceCollectionExtensions
{
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
        serviceCollection.AddTransient<Services.Downloader.YoutubeDownloader>();
        serviceCollection.AddSingleton<IDirectory>(_ =>
        {
            IDirectory dir = new CwdDirectory("Downloads");
            dir.Init();
            return dir;
        });
        serviceCollection.AddTransient<YoutubeClient>();
        serviceCollection.AddTransient<DownloadFactory<DownloadStatusContext>>();
        serviceCollection.AddSingleton<ConverterFactory<DownloadStatusContext>>();
        serviceCollection.AddSingleton<SystemInfo>();
        return serviceCollection;
    }

    public static IServiceCollection AddUpdaters(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUpdater, Updater.Noop>();
        serviceCollection.AddScoped<GitHubVersionClient>();
        serviceCollection.AddSingleton<TaggedVersion>(_ => new(1, 0, 4));
        serviceCollection.AddSingleton<FfmpegDownloader.Config>(new FfmpegConfigFactory(FfmpegDownloader.Config.Default)
            .ResolveConfig);
        serviceCollection.AddSingleton<FfmpegDownloader>();
        return serviceCollection;
    }
}