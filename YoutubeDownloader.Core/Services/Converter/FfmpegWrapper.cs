using System.Diagnostics;
using YoutubeDownloader.Core.Container;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class FfmpegAudioConversion
    : Stream
{
    private readonly Lazy<Process> _ffmpegProcess;

    private Stream Input => _ffmpegProcess.Value.StandardInput.BaseStream;

    public FfmpegAudioConversion(
        string ffmpegAbsolutePath,
        string outPath,
        IMediaContainer target,
        AudioMetadata metadata)
    {
        _ffmpegProcess
            = new Lazy<Process>(() => CreateProcess(ffmpegAbsolutePath, outPath, metadata, target));
    }

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

    private static List<string> Args(string outPath, AudioMetadata metadata, IMediaContainer target)
    {
        var args = new List<string>
        {
            "-nostdin",
            "-hide_banner",
            "-loglevel", "error",
            "-i", "pipe:0"
        };

        if (target.VideoStreamSupport is VideoStreamSupport.AttachedPic)
        {
            args.AddRange(
            [
                "-i", metadata.ThumbnailPath,
                "-c:v", "mjpeg",
                "-map", "1:v:0",
                "-disposition:v", "attached_pic",
            ]);
        }
        else
        {
            args.Add("-vn");
        }

        args.AddRange(
        [
            "-c:a", target.FfmpegCodec.FfmpegCodec,
            ..target.FfmpegCodecFlags.Format(),

            "-map", "0:a:0",

            "-map_metadata", "-1",
            "-metadata", $"title={metadata.Name}",
            "-metadata", $"artist={metadata.Author}",

            "-y",
            outPath
        ]);
        return args;
    }

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