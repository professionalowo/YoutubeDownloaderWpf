using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Setup.Settings;
using YoutubeDownloader.Wpf.View;

namespace YoutubeDownloader.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Page
{
    public Services.Downloader.YoutubeDownloader Downloader { get; private set; }
    private readonly IDownloadDirectoryFactory _downloadDirectoryFactory;
    private readonly ISettingsService _settingsService;

    public MainWindow(Services.Downloader.YoutubeDownloader downloader, IDownloadDirectoryFactory downloadDirectoryFactory, ISettingsService settingsService)
    {
        _downloadDirectoryFactory = downloadDirectoryFactory;
        Downloader = downloader;
        _settingsService = settingsService;
        DataContext = Downloader;
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e) => await Downloader.Download();

    private void Button_Click_Open_Downloads(object sender, RoutedEventArgs e) => _downloadDirectoryFactory.Create().Open();

    private async void Button_Click_1(object sender, RoutedEventArgs e)
        => await Downloader.Cancel();


}
