namespace YoutubeDownloader.Core.Data.Download;

public sealed record SingleVideoDownload(string Url) : AbstractVideoDownload(Url)
{
    public override string FormatName(string name) => name;
}