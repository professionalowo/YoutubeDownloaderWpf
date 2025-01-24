using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.InternalDirectory
{
    public class CwdDirectory : IDirectory
    {
        public CwdDirectory(string name)
        {
            FullPath = Path.Combine(Directory.GetCurrentDirectory(), name);
            Directory.CreateDirectory(FullPath);
        }
        public string FullPath { get; init; }
    }
}
