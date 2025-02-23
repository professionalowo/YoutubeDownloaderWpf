using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Services.AutoUpdater;

namespace YoutubeDownloaderWpf.Services.Converter;

public class FfmpegMp3Conversion(FfmpegDownloader.Config config, string outPath) : IDisposable, IAsyncDisposable
{
    private readonly Lazy<Process> _ffmpegProcess = new(() =>
    {
        var p = CreateProcess(config, outPath);
        p.Start();
        return p;
    });
    public Stream Input => _ffmpegProcess.Value.StandardInput.BaseStream;
    private static Process CreateProcess(FfmpegDownloader.Config config, string outPath)
    {
        return new()
        {
            StartInfo = new(config.FfmpegExeFullPath, GetArguments(outPath))
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
            }
        };
    }

    private static ICollection<string> GetArguments(string outPath) => [
        "-i", "pipe:0",
        "-vn",
        "-c:a", "libmp3lame",
        "-preset", "fast",
        "-threads", "3",
        "-map_metadata", "0:s:0",
        "-map_metadata", "0",
        "-q:a", "2",
        "-flush_packets", "0",
        "-y",
        outPath
    ];

    #region IDisposable
    private bool disposedValue;

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_ffmpegProcess.IsValueCreated)
                {
                    var p = _ffmpegProcess.Value;
                    Input.Flush();
                    Input.Close();
                    p.WaitForExit();
                    p.Dispose();
                }
            }
            disposedValue = true;
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_ffmpegProcess.IsValueCreated)
                {
                    var p = _ffmpegProcess.Value;
                    await Input.FlushAsync();
                    Input.Close();
                    await p.WaitForExitAsync();
                    p.Dispose();

                    disposedValue = true;
                }
            }
        }

    }
    #endregion
}
