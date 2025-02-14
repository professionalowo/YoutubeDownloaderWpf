using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Data
{
    public readonly struct DownloadData<T>(T data,DownloadStatusContext context)
    {
        public readonly T Data => data;
        public readonly DownloadStatusContext Context => context;
        internal void Deconstruct(out T d,out DownloadStatusContext c)
        {
            d = data;
            c = context;
        }
    }
}
