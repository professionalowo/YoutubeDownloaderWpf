using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeDownloaderWpf.Services.Downloader;
using YoutubeDownloaderWpf.Services.InternalDirectory;

namespace YoutubeDownloaderWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public YoutubeDownloader Downloader { get; private set; }
        private readonly IDirectory downloads;
        public MainWindow(YoutubeDownloader downloader, IDirectory downloads)
        {
            this.downloads = downloads;
            this.Downloader = downloader;
            this.DataContext = Downloader;
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e) => await Downloader.Download();

        private void Button_Click_Open_Downloads(object sender, RoutedEventArgs e) => downloads.Open();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Downloader.CancellationSources.ToList().ForEach(s => s.Cancel(false));
        }
    }
}
