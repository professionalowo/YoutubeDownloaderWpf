using System.Buffers;

namespace YoutubeDownloader.Core.Extensions;

public static class StreamExtensions
{
    // Ensure buffer size is a multiple of 4
    private const int buffer_size = 1024 * 64;

    private readonly struct AsyncConversion() : IAsyncDisposable, IDisposable
    {
        private readonly IMemoryOwner<byte> _buffer = MemoryPool<byte>.Shared.Rent(buffer_size);
        public required Stream Input { init; private get; }
        public required Stream Destination { init; private get; }

        public async Task Convert(IProgress<long> progress,
            CancellationToken token = default)
        {
            var memory = _buffer.Memory;
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await Input.ReadAsync(memory, token).ConfigureAwait(false)) > 0)
            {
                await Destination.WriteAsync(memory[..bytesRead], token).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                // Report progress
                progress?.Report(totalBytesRead);
            }
        }

        private class TrackedStream(Stream inner, IProgress<long> progress) : Stream
        {
            private Stream Inner => inner;

            public override void Flush()
            {
                Inner.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
                => Inner.Read(buffer, offset, count);

            public override long Seek(long offset, SeekOrigin origin) => Inner.Seek(offset, origin);

            public override void SetLength(long value)
                => Inner.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count)
            {
                Inner.Write(buffer, offset, count);
                progress.Report(count);
            }

            public override bool CanRead => Inner.CanRead;
            public override bool CanSeek => Inner.CanSeek;
            public override bool CanWrite => Inner.CanWrite;
            public override long Length => Inner.Length;

            public override long Position
            {
                get => Inner.Position;
                set => Inner.Position = value;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                Inner.Dispose();
            }

            public override async ValueTask DisposeAsync()
            {
                await base.DisposeAsync()
                    .ConfigureAwait(false);
                await Inner.DisposeAsync()
                    .ConfigureAwait(false);
            }
        }

        public void Dispose() => _buffer.Dispose();

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }

    public static async Task CopyToTrackedAsync(this Stream input, Stream destination, IProgress<long> progress,
        CancellationToken token = default)
    {
        await using var conversion = new AsyncConversion
        {
            Input = input,
            Destination = destination,
        };
        await conversion.Convert(progress, token).ConfigureAwait(false);
    }

    public static void CopyToTracked(this Stream input, Stream destination, IProgress<long> progress)
    {
        Span<byte> buffer = stackalloc byte[buffer_size]; // Buffer size: multiple of 4
        long totalBytesRead = 0;
        while (input.ReadOut(ref buffer, out var bytesRead))
        {
            destination.Write(buffer[..bytesRead]);
            totalBytesRead += bytesRead;
            // Report progress
            progress?.Report(totalBytesRead);
        }
    }

    private static bool ReadOut(this Stream s, ref Span<byte> b, out int r)
        => (r = s.Read(b)) > 0;
}