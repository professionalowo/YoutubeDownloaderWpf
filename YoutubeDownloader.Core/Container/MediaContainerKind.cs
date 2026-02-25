namespace YoutubeDownloader.Core.Container;

public abstract record MediaContainerKind : IMediaContainer
{
    public abstract string Extension { get; }
    public abstract string FfmpegCodec { get; }
    public virtual IEnumerable<string> FfmpegCodecFlags => [];

    private MediaContainerKind()
    {
        // Prevent external classes from inheriting this base
    }

    private sealed record Mp3 : MediaContainerKind
    {
        public override string ToString() => "Mp3 Audio";

        public override string Extension => "mp3";
        public override string FfmpegCodec => "libmp3lame";

        public override IEnumerable<string> FfmpegCodecFlags =>
        [
            "-q:a", "2",
            "-id3v2_version", "4",
        ];
    };

    private sealed record Wav : MediaContainerKind
    {
        public override string ToString() => "Wav Audio";
        public override string Extension => "wav";
        public override string FfmpegCodec => "pcm_s16le";

        public override IEnumerable<string> FfmpegCodecFlags => ["-ar", "48000", "-ac", "2"];
    };

    private sealed record Opus : MediaContainerKind
    {
        public override string ToString() => "Opus Audio";
        public override string Extension => "opus";
        public override string FfmpegCodec => "libopus";

        public override IEnumerable<string> FfmpegCodecFlags =>
            ["-b:a", "128k", "-vbr", "on", "-compression_level", "10"];
    }

    public static IReadOnlyList<IMediaContainer> All { get; } =
    [
        new Mp3(),
        new Wav(),
        new Opus()
    ];
}