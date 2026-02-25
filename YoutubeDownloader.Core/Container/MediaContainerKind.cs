namespace YoutubeDownloader.Core.Container;

internal sealed record Mp3 : IMediaContainer
{
    public override string ToString() => "Mp3 Audio";

    public IMediaContainer.FileExtension Extension => new("mp3");
    public IMediaContainer.Codec FfmpegCodec => new("libmp3lame");

    public IMediaContainer.Codec.Flags FfmpegCodecFlags =>
    [
        "-q:a", "2",
        "-id3v2_version", "4",
    ];
}

internal sealed record Wav : IMediaContainer
{
    public override string ToString() => "Wav Audio";
    public IMediaContainer.FileExtension Extension => new("wav");
    public IMediaContainer.Codec FfmpegCodec => new("pcm_s16le");

    public IMediaContainer.Codec.Flags FfmpegCodecFlags => ["-ar", "48000", "-ac", "2"];
}

internal sealed record Opus : IMediaContainer
{
    public override string ToString() => "Opus Audio";
    public IMediaContainer.FileExtension Extension => new("opus");
    public IMediaContainer.Codec FfmpegCodec => new("libopus");

    public IMediaContainer.Codec.Flags FfmpegCodecFlags => ["-b:a", "128k", "-vbr", "on", "-compression_level", "10"];
}

public static class MediaContainers
{
    public static IReadOnlyList<IMediaContainer> All { get; } =
    [
        new Mp3(),
        new Wav(),
        new Opus()
    ];
}