using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
<<<<<<< HEAD
<<<<<<< HEAD
using System.Threading;
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
using System.Threading.Tasks;
using Xabe.FFmpeg;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services.Converter
{
    public class Mp3Converter : IConverter
    {
<<<<<<< HEAD
<<<<<<< HEAD
        private object key { get; } = new object();
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
        public Mp3Converter()
        {
            FFmpeg.SetExecutablesPath("./ffmpeg");
        }
<<<<<<< HEAD
<<<<<<< HEAD

        public void RunConversion(string filePath, DownloadStatusContext context, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                lock (key)
                {
                    FileInfo fileInfo = new(filePath);
                    string outputFileName = Path.ChangeExtension(fileInfo.FullName, ".mp3");
                    Trace.WriteLine(fileInfo.FullName);
                    var conversion = FFmpeg.Conversions.FromSnippet.Convert(fileInfo.FullName, outputFileName).Result;
                    conversion.OnProgress += async (sender, args) =>
                    {
                        var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                        await Downloader.YoutubeDownloader.DispatchToUI(() => context.ProgressValue = percent);
                    };
                    Trace.WriteLine("Converting");
                    conversion.Start(token).ContinueWith(t => File.Delete(fileInfo.FullName), token).Wait();
                }
            }
            catch (OperationCanceledException) { };
=======
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
        public async Task RunConversion(string filePath, DownloadStatusContext context)
        {
            FileInfo fileInfo = new(filePath);
            string outputFileName = Path.ChangeExtension(fileInfo.FullName, ".mp3");
            Trace.WriteLine(fileInfo.FullName);
            var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileInfo.FullName, outputFileName);
            conversion.OnProgress += async (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                await Downloader.YoutubeDownloader.DispatchToUI(() => context.Progress = percent);
            };
            Trace.WriteLine("Converting");
            await conversion.Start().ContinueWith(t => File.Delete(fileInfo.FullName));
<<<<<<< HEAD
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
        }
    }
}
