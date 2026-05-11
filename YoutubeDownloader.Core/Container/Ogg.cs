namespace YoutubeDownloader.Core.Container;

public class Ogg() : AbstractMediaContainer("Ogg Vorbis Audio", "ogg", "libvorbis")
{
    public override VideoStreamSupport VideoStreamSupport => VideoStreamSupport.None;

    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("q:a", 4),
    ];
}