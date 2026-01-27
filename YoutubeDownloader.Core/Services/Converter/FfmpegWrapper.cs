using System.Diagnostics;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class FfmpegMp3Conversion<TContext>(string ffmpegAbsolutePath, string outPath, TContext context)
    : Stream where TContext : IConverter<TContext>.IConverterContext
{
    private readonly Lazy<Process> _ffmpegProcess
        = new(() => CreateProcess(ffmpegAbsolutePath, outPath, context));

    private Stream BaseInput => _ffmpegProcess.Value.StandardInput.BaseStream;

    private static Process CreateProcess(string ffmpegAbsolutePath, string outPath, TContext context)
    {
        var args = Args(outPath, context);
        var info = new ProcessStartInfo(ffmpegAbsolutePath, args)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            
        };
        return Process.Start(info) ??
               throw new InvalidOperationException($"Unable to create {ffmpegAbsolutePath} process.");
    }

    private static ICollection<string> Args(string outPath, TContext context) =>
    [
        "-thread_queue_size", "1024",
        "-i", "pipe:0",

        "-c:a", "libmp3lame",
        "-q:a", "2",
        "-id3v2_version", "4",

        "-vn",

        "-map_metadata", "-1",
        "-metadata", $"title={context.Name}",

        "-y",
        outPath
    ];

    public override void Flush()
        => BaseInput.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => BaseInput.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => BaseInput.Seek(offset, origin);

    public override void SetLength(long value)
        => BaseInput.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => BaseInput.Write(buffer, offset, count);

    public override bool CanRead => BaseInput.CanRead;
    public override bool CanSeek => BaseInput.CanSeek;
    public override bool CanWrite => BaseInput.CanWrite;
    public override long Length => BaseInput.Length;

    public override long Position
    {
        get => BaseInput.Position;
        set => BaseInput.Position = value;
    }

    #region IDisposable

    private bool _disposedValue;

    private bool ShouldDispose(bool disposing) => !_disposedValue && disposing && _ffmpegProcess.IsValueCreated;


    protected override void Dispose(bool disposing)
    {
        if (!ShouldDispose(disposing)) return;

        var p = _ffmpegProcess.Value;
        BaseInput.Flush();
        BaseInput.Close();
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
        await BaseInput.FlushAsync();
        BaseInput.Close();
        await p.WaitForExitAsync();
        p.Dispose();

        _disposedValue = true;
    }

    #endregion
}