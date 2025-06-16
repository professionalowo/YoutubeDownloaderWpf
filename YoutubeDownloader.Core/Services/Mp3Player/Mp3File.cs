namespace YoutubeDownloader.Core.Services.Mp3Player;

public record Mp3File(string FullPath)
{
    public string Name => Path.GetFileNameWithoutExtension(FullPath);
}