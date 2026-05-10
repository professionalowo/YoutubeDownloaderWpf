namespace YoutubeDownloader.Core.Container;

public static class MediaContainers
{
    private static IReadOnlyList<IMediaContainer> Containers { get; } =
    [
        new Mp3(),
        new Wav(),
        new Opus(),
        new Flac(),
        new Aac(),
        new Ogg()
    ];

    extension(IMediaContainer)
    {
        public static IReadOnlyList<IMediaContainer> All => Containers;
    }
}