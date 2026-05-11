namespace YoutubeDownloader.Core.Container;

internal sealed class Opus() : AbstractMediaContainer("Opus Audio", "opus", "libopus")
{
    public override VideoStreamSupport VideoStreamSupport => VideoStreamSupport.None;
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("b:a", "128k"),
        IMediaContainer.Codec.Flag.Create("vbr", "on"),
        IMediaContainer.Codec.Flag.Create("compression_level", 10)
    ];
}