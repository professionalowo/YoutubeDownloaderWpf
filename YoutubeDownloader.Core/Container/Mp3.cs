namespace YoutubeDownloader.Core.Container;

internal sealed class Mp3() : AbstractMediaContainer("Mp3 Audio", "mp3", "libmp3lame")
{
    public override IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        IMediaContainer.Codec.Flag.Create("q:a", 2),
        IMediaContainer.Codec.Flag.Create("id3v2_version", 4),
    ];
}