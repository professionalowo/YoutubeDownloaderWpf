using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Util.Extensions;

public static class StreamExtensions
{
    public static async Task CopyToAsyncTracked(this Stream input, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default)
    {
        // Ensure buffer size is a multiple of 4
        using var shared = MemoryPool<byte>.Shared.Rent(1024 * 1024);  // Buffer size: multiple of 4
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await input.ReadAsync(shared.Memory, cancellationToken).ConfigureAwait(false)) > 0)
        {
            
            await destination.WriteAsync(shared.Memory[..bytesRead], cancellationToken).ConfigureAwait(false);
            totalBytesRead += bytesRead;

            // Report progress
            progress?.Report(totalBytesRead);
        }
    }
    
    public static void CopyToTracked(this Stream input, Stream destination, IProgress<long> progress)
    {
        // Ensure buffer size is a multiple of 4
        Span<byte> buffer = stackalloc byte[1024 * 1024];  // Buffer size: multiple of 4
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = input.Read(buffer)) > 0)
        {
            
            destination.Write(buffer[..bytesRead]);
            totalBytesRead += bytesRead;
            // Report progress
            progress?.Report(totalBytesRead);
        }
    }
}
