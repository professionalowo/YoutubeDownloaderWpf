using System.Windows;
using YoutubeDownloader.Core.Services.InternalDirectory;

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
