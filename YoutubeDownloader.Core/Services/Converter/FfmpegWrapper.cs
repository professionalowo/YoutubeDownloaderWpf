using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public class FfmpegMp3Conversion(string ffmpegAbsolutePath, string outPath) : IDisposable, IAsyncDisposable
{
    private readonly Lazy<Process> _ffmpegProcess
        = new(() => CreateProcess(ffmpegAbsolutePath, outPath));

    public Stream Input => _ffmpegProcess.Value.StandardInput.BaseStream;

    private static Process CreateProcess(string ffmpegAbsolutePath, string outPath)
    {
        return AsStarted(new Process
        {
            StartInfo = new ProcessStartInfo(ffmpegAbsolutePath, GetArguments(outPath))
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
            }
        });
    }

    private static ICollection<string> GetArguments(string outPath) =>
    [
        "-i", "pipe:0",
        "-vn",
        "-c:a", "libmp3lame",
        "-preset", "fast",
        "-map_metadata", "0:s:0",
        "-map_metadata", "0",
        "-q:a", "2",
        "-flush_packets", "0",
        "-y",
        outPath
    ];

    private static Process AsStarted(Process process)
    {
        process.Start();
        return process;
    }

    #region IDisposable

    private bool _disposedValue;

    private bool ShouldDispose(bool disposing) => !_disposedValue && disposing && _ffmpegProcess.IsValueCreated;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!ShouldDispose(disposing)) return;

        var p = _ffmpegProcess.Value;
        Input.Flush();
        Input.Close();
        p.WaitForExit();
        p.Dispose();

        _disposedValue = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (!ShouldDispose(disposing)) return;

        var p = _ffmpegProcess.Value;
        await Input.FlushAsync();
        Input.Close();
        await p.WaitForExitAsync();
        p.Dispose();

        _disposedValue = true;
    }

    #endregion
}