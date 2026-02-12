using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class FfmpegMp3Conversion<TContext>(
    string ffmpegAbsolutePath,
    string outPath,
    TContext context)
    : Stream where TContext : IConverter<TContext>.IConverterContext
{
    private readonly Lazy<Process> _ffmpegProcess
        = new(() => CreateProcess(ffmpegAbsolutePath, outPath, context));

    private Stream Input => _ffmpegProcess.Value.StandardInput.BaseStream;

    private static Process CreateProcess(string ffmpegAbsolutePath, string outPath, TContext context)
    {
        var args = Args(outPath, context);
        var info = new ProcessStartInfo(ffmpegAbsolutePath, args)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
        };
        var process = Process.Start(info)!;
        return process;
    }

    private static ICollection<string> Args(string outPath, TContext context) =>
    [
        "-nostdin",
        "-hide_banner",
        "-loglevel", "error",
        "-i", "pipe:0",

        "-c:a", "libmp3lame",
        "-q:a", "2",
        "-id3v2_version", "4",

        "-vn",

        "-map", "0:a:0?",
        "-map_metadata", "-1",
        "-metadata", $"title={context.Name}",

        "-y",
        outPath
    ];

    public override void Flush()
        => Input.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => Input.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => Input.Write(buffer, offset, count);

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    private bool _disposedValue;

    private bool ShouldDispose(bool disposing) => !_disposedValue && disposing && _ffmpegProcess.IsValueCreated;


    protected override void Dispose(bool disposing)
    {
        if (!ShouldDispose(disposing)) return;

        var p = _ffmpegProcess.Value;
        Input.Flush();
        Input.Close();
        p.WaitForExit();
        p.Dispose();

        _disposedValue = true;
    }

    public override async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
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
}