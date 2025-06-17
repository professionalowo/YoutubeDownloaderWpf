using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Core.Services.Mp3Player;

public class Mp3Player(IDirectory downloads, FfmpegDownloader.Config config) : INotifyPropertyChanged
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