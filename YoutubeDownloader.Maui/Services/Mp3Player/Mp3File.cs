using System.IO;
using CommunityToolkit.Maui.Views;

namespace YoutubeDownloader.Maui.Services.Mp3Player;

public sealed record Mp3File(string FullPath)
{
    public string Name => Path.GetFileNameWithoutExtension(FullPath);
    public MediaSource Source => MediaSource.FromFile(FullPath);
}