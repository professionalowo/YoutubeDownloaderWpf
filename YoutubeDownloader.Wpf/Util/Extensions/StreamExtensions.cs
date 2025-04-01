using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Util.Extensions;

public static class StreamExtensions
{
    public static async Task CopyToAsyncTracked(this Stream input, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default)
    {
        Memory<byte> buffer = new byte[1024 * 1000];
        long totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = await input.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer[..bytesRead], cancellationToken);
            totalBytesRead += bytesRead;

            // Report progress (totalBytesRead represents the total amount of data copied)
            progress?.Report(totalBytesRead);
        }
    }

}
