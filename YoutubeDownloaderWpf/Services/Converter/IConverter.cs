using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services.Converter
{
    public interface IConverter
    {
        public Task Convert(Stream data, string outPath, DownloadStatusContext context, CancellationToken token = default);
    }
}
