using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using YoutubeDownloaderWpf.Services.Downloader;
using YoutubeDownloaderWpf.Services.Logging;

namespace YoutubeDownloaderWpf
{
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
            serviceCollection.AddTransient<YoutubeDownloader>();
            serviceCollection.AddTransient<MainWindow>();
            serviceCollection.AddLogging(builder =>
                    builder.AddProvider(new FileLoggerProvider("logs.txt"))
                    .SetMinimumLevel(LogLevel.Warning)
            );
            return serviceCollection.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _mainWindow = services.GetService<MainWindow>();
            _mainWindow?.Show();
        }
    }
}
