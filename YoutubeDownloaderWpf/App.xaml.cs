﻿using Microsoft.Extensions.DependencyInjection;
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
using YoutubeDownloaderWpf.Services.AutoUpdater;
using YoutubeDownloaderWpf.Services.Converter;
using YoutubeDownloaderWpf.Services.Downloader;
using YoutubeDownloaderWpf.Services.Downloader.Download;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Services.Logging;
using YoutubeDownloaderWpf.Util;
using YoutubeExplode;

namespace YoutubeDownloaderWpf;

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
        logger?.LogError(e.Exception.ToString());
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
        var updater = services.GetService<Updater>()!;
        bool isNewVersion = await updater.IsNewVersionAvailable();
        if (isNewVersion)
        {
            await updater.UpdateVersion();
        }
        var ffmpeg = services.GetService<FfmpegDownloader>()!;
        await ffmpeg.DownloadFfmpeg();
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
        serviceCollection.AddTransient<YoutubeDownloader>();
        serviceCollection.AddSingleton<IDirectory>(_ =>
        {
            IDirectory dir = new CwdDirectory("Downloads");
            dir.Init();
            return dir;
        });
        serviceCollection.AddTransient<YoutubeClient>();
        serviceCollection.AddTransient<DownloadFactory>();
        serviceCollection.AddSingleton<ConverterFactory>();
        serviceCollection.AddSingleton<SystemInfo>();
        return serviceCollection;
    }

    public static IServiceCollection AddUpdaters(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<Updater>();
        serviceCollection.AddScoped<GitHubVersionClient>();
        serviceCollection.AddSingleton<Updater.Version>(_ => new(1, 0, 4));
        serviceCollection.AddSingleton<FfmpegDownloader.Config>(FfmpegConfigFactory.ResolveConfig);
        serviceCollection.AddSingleton<FfmpegDownloader>();
        return serviceCollection;
    }
}
