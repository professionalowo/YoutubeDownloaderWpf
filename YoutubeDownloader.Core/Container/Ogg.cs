namespace YoutubeDownloader.Core.Container;

public class Ogg() : AbstractMediaContainer("Ogg Vorbis Audio", "ogg", "libvorbis")
{
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("q:a", 4),
    ];
}