using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Maui;

using Microsoft.Maui.Controls;

public partial class MainPage : ContentPage
{
    private readonly Services.YoutubeDownloader _downloader;
    private readonly IDirectory _downloads;

    public MainPage(Services.YoutubeDownloader downloader, IDirectory downloads)
    {
        _downloader = downloader;
        _downloads = downloads;
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
        _downloads.Open();
    }
}