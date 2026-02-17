namespace YoutubeDownloader.Core.Extensions;

public static class ProgressExtensions
{
    extension<T>(IProgress<T>)
    {
        public static IProgress<T> Null => new Progress<T>(_ => { });
    }
}