namespace YoutubeDownloader.Core.Container;

public interface IMediaContainer
{
    string Extension { get; }
    string FfmpegCodec { get; }
    ICollection<string> FfmpegCodecFlags { get; }
}