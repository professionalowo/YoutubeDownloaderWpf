using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.Mp3Player;

namespace YoutubeDownloader.Maui;

public partial class PlayerPage : ContentPage
{
    private readonly Mp3Player player;

    public PlayerPage(Mp3Player player)
    {
        this.player = player;
        BindingContext = this.player;
        InitializeComponent();
    }
}