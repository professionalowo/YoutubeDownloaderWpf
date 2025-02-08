using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater
{
    public class FfmpegDownloader(ILogger<FfmpegDownloader> logger, HttpClient client, FfmpegDownloader.Config config)
    {
        public async Task DownloadFfmpeg()
        {
        }


        public class Config(string folder = "./ffmpeg", string ffmpegExeName = "ffmpeg", string ffprobeExeName = "ffprobe")
        {
            public string Folder => folder;
            public string FfmpegExeName => ffmpegExeName;
            public string FfprobeExeName => ffprobeExeName;
        }
    }
}
