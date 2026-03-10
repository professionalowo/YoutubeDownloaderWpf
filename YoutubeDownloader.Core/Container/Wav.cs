namespace YoutubeDownloader.Core.Container;

internal sealed class Wav() : AbstractMediaContainer("Wav Audio", "wav", "pcm_s16le")
{
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("ar", 48000),
        IMediaContainer.Codec.Flag.Create("ac", 2)
    ];
}