namespace YoutubeDownloader.Core.Extensions;

public static class ProgressExtensions
{
    extension<T>(IProgress<T>)
    {
        public static IProgress<T> Null => new NullProgress<T>();
    }
}

internal readonly struct NullProgress<T> : IProgress<T>
{
    public void Report(T value)
    {
    }
}