namespace YoutubeDownloader.Core.Data.Download;

public readonly record struct NamedVideoDownload(IVideoDownload Download, string Title, string Author);