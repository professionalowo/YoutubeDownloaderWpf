using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Data
{
    public readonly struct DownloadData(string path,DownloadStatusContext context)
    {
        public readonly string Path => path;
        public readonly DownloadStatusContext Context => context;
        internal void Deconstruct(out string p,out DownloadStatusContext c)
        {
            p = path;
            c = context;
        }
    }
}
