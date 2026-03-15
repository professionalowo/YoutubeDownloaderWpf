namespace YoutubeDownloader.Core.Extensions;

internal sealed class SingleAsyncEnumerable<T>(T item) : IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new SingleAsyncEnumerator(item, cancellationToken);


    private sealed class SingleAsyncEnumerator(T item, CancellationToken token = default) : IAsyncEnumerator<T>
    {
        private bool _consumed;

        public T Current => item;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync()
        {
            if (token.IsCancellationRequested) return ValueTask.FromCanceled<bool>(token);
            if (_consumed) return ValueTask.FromResult(false);

            _consumed = true;
            return ValueTask.FromResult(true);
        }
    }
}