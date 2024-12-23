using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.InternalDirectory
{
    public class DownloadDirectory : IDirectory
    {
        public DownloadDirectory() {
            Directory.CreateDirectory(FullPath);
        }
        public string FullPath => Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
    }
}
