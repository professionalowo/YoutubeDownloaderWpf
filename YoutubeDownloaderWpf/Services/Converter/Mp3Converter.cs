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
    public class Mp3Converter(FfmpegDownloader.Config config)
    {
        public async Task ConvertToMp3File(Stream data, string filePath,DownloadStatusContext context,CancellationToken token = default) {
            await using FfmpegMp3Conversion conversion = new(config, filePath);
            await data.CopyToAsyncTracked(conversion.Input, GetProgressWrapper(context), token);
            context.InvokeDownloadFinished(this, true);
        }

        private static Progress<long> GetProgressWrapper(DownloadStatusContext context)
        {
            Progress<long> downloadProgress = new();
            downloadProgress.ProgressChanged += (_, e) => {
                var percentage = Math.Min(e / (context.Size * 1000), 100);
                if (context.ProgressHandler is IProgress<double> p)
                {
                    p.Report(percentage);
                }
            };

            return downloadProgress;
        }
    }
}
