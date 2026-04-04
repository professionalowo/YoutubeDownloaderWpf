using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Maui;

using Microsoft.Maui.Controls;

public partial class MainPage : ContentPage
{
    private readonly Services.YoutubeDownloader _downloader;
    private readonly IDownloadDirectoryFactory _downloadDirectoryFactory;
    private readonly ISettingsService _settingsService;

    public MainPage(Services.YoutubeDownloader downloader, IDownloadDirectoryFactory downloadDirectoryFactory, ISettingsService settingsService)
    {
        _downloader = downloader;
        _downloadDirectoryFactory = downloadDirectoryFactory;
        _settingsService = settingsService;
        BindingContext = downloader;
        InitializeComponent();
    }

    private async void Download_OnClicked(object? sender, EventArgs e)
    {
        await _downloader.Download()
            .ConfigureAwait(false);
    }

    private void Open_OnClicked(object? sender, EventArgs e)
    {
        _downloadDirectoryFactory.Create().Open();
    }


}