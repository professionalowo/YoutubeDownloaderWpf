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
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Wpf.Services.Downloader;

namespace YoutubeDownloader.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Services.Downloader.YoutubeDownloader Downloader { get; private set; }
    private readonly IDirectory downloads;
    public MainWindow(Services.Downloader.YoutubeDownloader downloader, IDirectory downloads)
    {
        this.downloads = downloads;
        this.Downloader = downloader;
        this.DataContext = Downloader;
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e) => await Downloader.Download();

    private void Button_Click_Open_Downloads(object sender, RoutedEventArgs e) => downloads.Open();

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    => await Downloader.Cancel();

}
