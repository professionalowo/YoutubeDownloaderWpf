namespace YoutubeDownloader.Core.Container;

public abstract record MediaContainerKind : IMediaContainer
{
    public abstract string Extension { get; }

    private MediaContainerKind()
    {
        // Prevent external classes from inheriting this base
    }

    public sealed record Mp3 : MediaContainerKind
    {
        public override string Extension => "mp3";
        public override string ToString() => "Mp3 Audio";
    };

    public sealed record Wav : MediaContainerKind
    {
        public override string Extension => "wav";
        public override string ToString() => "Wav Audio";
    };

    public static IReadOnlyList<IMediaContainer> All { get; } =
    [
        new Mp3(),
        new Wav()
    ];
}