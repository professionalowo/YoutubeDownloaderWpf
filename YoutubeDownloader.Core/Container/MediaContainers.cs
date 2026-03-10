namespace YoutubeDownloader.Core.Container;

public static class MediaContainers
{
    public static IReadOnlyList<IMediaContainer> All { get; } =
    [
        new Mp3(),
        new Wav(),
        new Opus(),
        new Flac()
    ];
}