namespace YoutubeDownloader.Core.Util;

public sealed class TempFile(FileInfo file) : IDisposable
{
    public string FilePath => file.FullName;

    public void Dispose()
    {
        if (file.Exists)
        {
            file.Delete();
        }
    }

    public static TempFile Create()
    {
        var path = Path.GetTempFileName();
        return new TempFile(new FileInfo(path));
    }

    public Task WriteAllBytesAsync(ReadOnlyMemory<byte> bytes, CancellationToken token = default)
        => File.WriteAllBytesAsync(FilePath, bytes, token);
}