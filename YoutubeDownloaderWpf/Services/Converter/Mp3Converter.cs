﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Services.Downloader;

namespace YoutubeDownloaderWpf.Services.Converter
{
    public class Mp3Converter : IConverter
    {
        public Mp3Converter()
        {
            FFmpeg.SetExecutablesPath("./ffmpeg");
        }

        public async Task RunConversion(string filePath, DownloadStatusContext context, CancellationToken token = default)
        {
            FileInfo fileInfo = new(filePath);
            string outputFileName = Path.ChangeExtension(fileInfo.FullName, ".mp3");
            Trace.WriteLine(fileInfo.FullName);
            var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileInfo.FullName, outputFileName);
            conversion.AddParameter("-map_metadata 0");
            conversion.AddParameter("-map_metadata 0:s:0");
            conversion.SetAudioBitrate(96000);
            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                YoutubeDownloader.DispatchToUISync(() => context.ProgressValue = percent);
            };
            Trace.WriteLine("Converting");
            await conversion.Start(token).ContinueWith(t => File.Delete(fileInfo.FullName), token);
        }
    }
}
