using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloaderWpf.Services.Downloader;
using YoutubeDownloaderWpf.Services.Logging;

namespace YoutubeDownloaderWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider services;
        public App()
        {
            services = InitializeServices();
        }

        private static ServiceProvider InitializeServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDownloader, YoutubeDownloader>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddLogging(builder =>
                builder.AddProvider(new FileLoggerProvider("logs.txt"))
            );
            return serviceCollection.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            services.GetService<MainWindow>()?.Show();
        }
    }
}
