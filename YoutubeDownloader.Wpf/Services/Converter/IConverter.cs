using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Wpf.Controls;

namespace YoutubeDownloader.Wpf.Services.Converter;

public interface IConverter
{
    public ValueTask<string?> Convert(Stream data, string outPath, DownloadStatusContext context, CancellationToken token = default);
}
