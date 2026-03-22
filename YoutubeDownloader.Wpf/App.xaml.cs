using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Threading;
using YoutubeDownloader.Setup;
using Velopack;
using Velopack.Sources;
using YoutubeDownloader.Core.Services.AutoUpdater;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services.Logging;
using YoutubeDownloader.Wpf.Services;
using YoutubeDownloader.Wpf.View;

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
        ShutdownMode = ShutdownMode.OnMainWindowClose;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = services.GetService<ILogger<App>>();
        logger?.LogError("{Error}", e.Exception);
    }

    private static ServiceProvider InitializeServices()
    {
        var serviceCollection = new ServiceCollection();
        var manager =
            new UpdateManager(new GithubSource(GitHubVersion.url, null, false));

        IDirectory root = manager.GetBasePath() is { } path
            ? new AbsoluteDirectory(path)
            : new CwdDirectory(".");

        serviceCollection.AddTransient<UpdateManager>(_ => manager)
            .AddTransient<VelopackService>()
            .AddConfig(root)
            .AddDownloadServices<Services.Downloader.YoutubeDownloader>(root)
            .AddTransient<MainWindow>();
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

        _mainWindow = services.GetService<MainWindow>();

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

        _mainWindow?.Show();
    }
}