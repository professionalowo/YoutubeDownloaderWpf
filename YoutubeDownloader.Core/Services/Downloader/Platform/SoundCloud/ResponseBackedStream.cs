namespace YoutubeDownloader.Core.Services.Downloader.Platform.SoundCloud;

internal sealed class ResponseBackedStream : Stream
{
    private bool _disposed;

    private readonly HttpResponseMessage _response;
    private readonly Stream _inner;

    private ResponseBackedStream(Stream inner, HttpResponseMessage response)
        => (_inner, _response) = (inner, response);


    public override bool CanRead => !_disposed && _inner.CanRead;
    public override bool CanSeek => !_disposed && _inner.CanSeek;
    public override bool CanWrite => false;
    public override long Length => _inner.Length;

    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }

    public override void Flush() => _inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => _inner.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => _inner.Seek(offset, origin);

    public override void SetLength(long value)
        => _inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public static async Task<ResponseBackedStream> CreateAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var inner = await response.Content.ReadAsStreamAsync();
        return new ResponseBackedStream(inner, response);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _inner.Dispose();
            _response.Dispose();
        }

        _disposed = true;
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await _inner.DisposeAsync();
        _response.Dispose();
        _disposed = true;
        await base.DisposeAsync();
    }
}