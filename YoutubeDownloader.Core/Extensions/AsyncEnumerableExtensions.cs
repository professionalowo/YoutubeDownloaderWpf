using System.Runtime.CompilerServices;

namespace YoutubeDownloader.Core.Extensions;

public static class AsyncEnumerableExtensions
{
    extension<T>(AsyncEnumerable)
    {
        public static async IAsyncEnumerable<T> FromSingle(T item,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            yield return item;
            await Task.CompletedTask
                .ConfigureAwait(false);
        }
    }
}