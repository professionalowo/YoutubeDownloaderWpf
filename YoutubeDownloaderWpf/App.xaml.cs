﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloaderWpf.Services.Downloader;

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
            serviceCollection.AddSingleton<IDownloader,YoutubeDownloader>();
            serviceCollection.AddSingleton<MainWindow>();
            return serviceCollection.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            services.GetService<MainWindow>()?.Show();
        }
    }
}
