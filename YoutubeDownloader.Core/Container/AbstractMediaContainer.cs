namespace YoutubeDownloader.Core.Container;

public abstract class AbstractMediaContainer(string display, string extension, string codec) : IMediaContainer
{
    public override string ToString() => display;
    public IMediaContainer.FileExtension Extension { get; } = new(extension);
    public IMediaContainer.Codec FfmpegCodec { get; } = new(codec);
    public virtual IMediaContainer.Codec.Flags FfmpegCodecFlags { get; } = [];
}