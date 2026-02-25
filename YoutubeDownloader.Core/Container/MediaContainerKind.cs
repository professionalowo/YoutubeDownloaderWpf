namespace YoutubeDownloader.Core.Container;

internal sealed record Mp3 : IMediaContainer
{
    public override string ToString() => "Mp3 Audio";

    public string Extension => "mp3";
    public string FfmpegCodec => "libmp3lame";

    public ICollection<string> FfmpegCodecFlags =>
    [
        "-q:a", "2",
        "-id3v2_version", "4",
    ];
}

internal sealed record Wav : IMediaContainer
{
    public override string ToString() => "Wav Audio";
    public string Extension => "wav";
    public string FfmpegCodec => "pcm_s16le";

    public ICollection<string> FfmpegCodecFlags => ["-ar", "48000", "-ac", "2"];
}

internal sealed record Opus : IMediaContainer
{
    public override string ToString() => "Opus Audio";
    public string Extension => "opus";
    public string FfmpegCodec => "libopus";

    public ICollection<string> FfmpegCodecFlags => ["-b:a", "128k", "-vbr", "on", "-compression_level", "10"];
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