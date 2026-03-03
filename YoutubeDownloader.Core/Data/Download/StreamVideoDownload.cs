using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Data.Download;

public record StreamVideoDownload(IVideoDownload Download, IStreamInfo Info)
{
    public double SizeInMb => Info.Size.MegaBytes;
}