using System.Diagnostics;
using YoutubeDownloader.Core.Container;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class FfmpegAudioConversion(
    string ffmpegAbsolutePath,
    string outPath,
    IMediaContainer target,
    AudioMetadata metadata)
    : Stream
{
    private readonly Lazy<Process> _ffmpegProcess
        = new(() => CreateProcess(ffmpegAbsolutePath, outPath, metadata, target));

    private Stream Input => _ffmpegProcess.Value.StandardInput.BaseStream;

    private static Process CreateProcess(string ffmpegAbsolutePath, string outPath, AudioMetadata metadata,
        IMediaContainer target)
    {
        var args = Args(outPath, metadata, target);
        var info = new ProcessStartInfo(ffmpegAbsolutePath, args)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
        };
        return Process.Start(info)!;
    }

    private static ICollection<string> Args(string outPath, AudioMetadata metadata, IMediaContainer target) =>
    [
        "-nostdin",
        "-hide_banner",
        "-loglevel", "error",
        "-i", "pipe:0",

        "-c:a", target.FfmpegCodec.FfmpegCodec,
        ..target.FfmpegCodecFlags,

        "-vn",

        "-map", "0:a:0?",
        "-map_metadata", "-1",
        "-metadata", $"title={metadata.Name}",

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
        await Input.FlushAsync()
            .ConfigureAwait(false);
        Input.Close();
        await p.WaitForExitAsync()
            .ConfigureAwait(false);

        p.Dispose();

        _disposedValue = true;
    }
}