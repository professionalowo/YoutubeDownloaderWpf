namespace YoutubeDownloader.Core.Data.Download;

public record StreamVideoDownload(IVideoDownload Download, IPlatformStreamInfo Info)
{
    public double SizeInMb => Info.SizeInMb;
}