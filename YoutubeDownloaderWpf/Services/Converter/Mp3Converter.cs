using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Services.AutoUpdater;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Util.Extensions;

namespace YoutubeDownloaderWpf.Services.Converter
{
    public class Mp3Converter(FfmpegDownloader.Config config) : IConverter
    {
        public ValueTask Convert(Stream data, string outPath, DownloadStatusContext context, CancellationToken token)
            => ConvertToMp3File(data, outPath, context, token);

        public async ValueTask ConvertToMp3File(Stream data, string filePath, DownloadStatusContext context, CancellationToken token = default)
        {
            try
            {
                await using FfmpegMp3Conversion conversion = new(config, filePath);
                await data.CopyToAsyncTracked(conversion.Input, context.GetProgressWrapper(), token);
                context.InvokeDownloadFinished(this, true);
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                context.InvokeDownloadFinished(this, false);
            }
        }

    }
}
