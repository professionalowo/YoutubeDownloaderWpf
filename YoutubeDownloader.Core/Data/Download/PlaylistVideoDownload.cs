namespace YoutubeDownloader.Core.Data.Download;

public sealed record PlaylistVideoDownload(string PlaylistName, string Url) : AbstractVideoDownload(Url)
{
    public override string FormatName(string name)
        => Path.Join(PlaylistName, name);
}