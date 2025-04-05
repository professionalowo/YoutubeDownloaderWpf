using System;
using Microsoft.Maui.Accessibility;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Maui;
using Microsoft.Maui.Controls;
public partial class MainPage : ContentPage
{
    private readonly Services.YoutubeDownloader downloader;
    public MainPage(Services.YoutubeDownloader downloader)
    {
        this.downloader = downloader;
        BindingContext = downloader;
        InitializeComponent();
    }
}