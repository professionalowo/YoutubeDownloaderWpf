using System.Buffers;

namespace YoutubeDownloader.Core.Extensions;

public static class StreamExtensions
{
    private const int buffer_size = 1024 * 64;

    public static async Task CopyToTrackedAsync(this Stream input, Stream destination, IProgress<long> progress,
        CancellationToken token = default)
    {
        // Ensure buffer size is a multiple of 4
        using var shared = MemoryPool<byte>.Shared.Rent(buffer_size); // Buffer size: multiple of 4
        var memory = shared.Memory;
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await input.ReadAsync(memory, token).ConfigureAwait(false)) > 0)
        {
            await destination.WriteAsync(memory[..bytesRead], token).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            // Report progress
            progress?.Report(totalBytesRead);
        }
    }

    public static void CopyToTracked(this Stream input, Stream destination, IProgress<long> progress)
    {
        // Ensure buffer size is a multiple of 4
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