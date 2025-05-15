﻿namespace YoutubeDownloader.Core.Extensions;

public static class StreamExtensions
{
    public static Stream Tracked(this Stream inner, IProgress<long> progress)
        => new TrackedStream(inner, progress);
}

internal class TrackedStream(Stream inner, IProgress<long> progress) : Stream
{
    public override void Flush()
        => inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => inner.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => inner.Seek(offset, origin);

    public override void SetLength(long value)
        => inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
        progress.Report(count);
    }

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanWrite => inner.CanWrite;
    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }
}