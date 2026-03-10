namespace YoutubeDownloader.Core.Container;

internal sealed class Flac() : AbstractMediaContainer("Flac Audio", "flac", "flac")
{
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("compression_level", 5),
        IMediaContainer.Codec.Flag.Create("sample_fmt", "s32"),
        IMediaContainer.Codec.Flag.Create("compression_level", 12),
        IMediaContainer.Codec.Flag.Create("ar", 96000),
    ];
}