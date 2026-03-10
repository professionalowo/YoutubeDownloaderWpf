namespace YoutubeDownloader.Core.Container;

internal sealed class Aac() : AbstractMediaContainer("AAC Audio (M4A)", "m4a", "aac")
{
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("b:a", "192k")
    ];
}