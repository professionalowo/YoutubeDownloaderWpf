using System.Windows;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Wpf.View;

namespace YoutubeDownloader.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Services.Downloader.YoutubeDownloader Downloader { get; private set; }
    private readonly IDirectory _downloads;
    private readonly ISettingsService _settingsService;

    public MainWindow(Services.Downloader.YoutubeDownloader downloader, IDirectory downloads, ISettingsService settingsService)
    {
        _downloads = downloads;
        Downloader = downloader;
        _settingsService = settingsService;
        DataContext = Downloader;
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e) => await Downloader.Download();

    private void Button_Click_Open_Downloads(object sender, RoutedEventArgs e) => _downloads.Open();

    private async void Button_Click_1(object sender, RoutedEventArgs e)
        => await Downloader.Cancel();

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settingsService);
        settingsWindow.ShowDialog();
    }
}
