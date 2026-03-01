namespace YoutubeDownloader.Core.Data.Download;

public interface IVideoDownload
{
    public string Url { get; }

    public string FormatName(string name);
}