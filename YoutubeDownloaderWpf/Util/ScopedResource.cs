using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Util.ScopedResource
{
    public abstract class ScopedResource : IDisposable
    {
        protected readonly string _path;
        public string FullPath => _path;
        private bool disposedValue;
        public ScopedResource(string path)
        {
            _path = path;
        }



        protected abstract void CleanResource();

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && Path.Exists(FullPath))
                {
                    // TODO: dispose managed state (managed objects)
                    CleanResource();
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public class File(string path) : ScopedResource(path)
        {
            protected override void CleanResource()
            => System.IO.File.Delete(path);
        }

        public class Directory(string path) : ScopedResource(path)
        {
            protected override void CleanResource()
            => System.IO.Directory.Delete(path, true);
        }
    }
}
