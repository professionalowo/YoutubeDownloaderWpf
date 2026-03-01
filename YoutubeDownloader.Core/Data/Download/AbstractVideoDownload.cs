namespace YoutubeDownloader.Core.Data.Download;

public abstract record AbstractVideoDownload(string Url) : IVideoDownload
{
    public abstract string FormatName(string name);
}