namespace YoutubeDownloader.Core.Extensions;

public static class AsyncEnumerableExtensions
{
    extension<T>(AsyncEnumerable)
    {
        public static IAsyncEnumerable<T> FromSingle(T item)
            => new SingleAsyncEnumerable<T>(item);
    }
}