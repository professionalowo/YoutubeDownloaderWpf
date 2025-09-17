namespace YoutubeDownloader.Core.Util;

public abstract class ScopedResource(string path) : IDisposable
{
    public string FullPath => path;
    private bool _disposedValue;

    protected abstract void CleanResource();

    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing && Path.Exists(FullPath))
        {
            CleanResource();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public sealed class File(string path) : ScopedResource(path)
    {
        protected override void CleanResource()
            => System.IO.File.Delete(FullPath);
    }

    public sealed class Directory(string path) : ScopedResource(path)
    {
        protected override void CleanResource()
            => System.IO.Directory.Delete(FullPath, true);
    }
}