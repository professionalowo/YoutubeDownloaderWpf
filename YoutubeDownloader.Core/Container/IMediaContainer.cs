namespace YoutubeDownloader.Core.Container;

public interface IMediaContainer
{
    string Extension { get; }
    string FfmpegCodec { get; }
    IEnumerable<string> FfmpegCodecFlags { get; }
}