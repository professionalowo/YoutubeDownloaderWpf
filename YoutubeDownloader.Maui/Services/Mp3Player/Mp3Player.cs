using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Views;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Maui.Services.Mp3Player;

public sealed partial class Mp3Player(IDirectory downloads) : INotifyPropertyChanged
{
    private const string searchFilter = "*.mp3";
    
    public Mp3File? SelectedFile
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public ICollection<Mp3File> Files =>
    [
        ..downloads.GetFiles(searchFilter)
            .Select(p => new Mp3File(p))
    ];

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}