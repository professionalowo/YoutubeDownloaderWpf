using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloaderWpf.Services.InternalDirectory;

namespace YoutubeDownloaderWpf.Services.AutoUpdater
{
    public class FfmpegDownloader(ILogger<FfmpegDownloader> logger, HttpClient client, FfmpegDownloader.Config config)
    {
        public async ValueTask<bool> DownloadFfmpeg()
        {
            string? zipSource = null;
            string? sourceUnzipped = null;
            if (DoesFfmpegExist(config)) return true;
            try
            {


                Directory.CreateDirectory(config.FfmpegFolder.FullPath);

                var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Downlaod Ffmpeg", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (res != MessageBoxResult.OK) return false;

                using var response = await client.GetAsync(Config.Source);
                using var readStream = await response.Content.ReadAsStreamAsync();

                zipSource = config.FfmpegFolder.SaveFileName("source.zip");
                sourceUnzipped = config.FfmpegFolder.SaveFileName(Path.GetFileNameWithoutExtension(zipSource));

                using (var fileStream = new FileStream(zipSource, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
                {
                    readStream.CopyTo(fileStream);
                }


                ZipFile.ExtractToDirectory(zipSource, sourceUnzipped);
                string[] executables = Directory.GetFiles(sourceUnzipped, "*.exe", SearchOption.AllDirectories);

                static string exeAppender(string path) => path + ".exe";

                string ffmpegExe = exeAppender(config.FfmpegExeName);
                string ffprobeExe = exeAppender(config.FfprobeExeName);

                string ffmpegPathSource = executables.Where(path => path.EndsWith(ffmpegExe)).First();
                string ffprobePathSource = executables.Where(path => path.EndsWith(ffprobeExe)).First();

                File.Move(ffmpegPathSource, config.FfmpegFolder.SaveFileName(ffmpegExe));
                File.Move(ffprobePathSource, config.FfmpegFolder.SaveFileName(ffprobeExe));

            }
            finally
            {
                if (zipSource != null && File.Exists(zipSource))
                    File.Delete(zipSource);
                if (sourceUnzipped != null && Directory.Exists(sourceUnzipped))
                    Directory.Delete(sourceUnzipped, true);
            }
            return true;
        }

        private static bool DoesFfmpegExist(Config config)
        {
            var directory = config.FfmpegFolder;
            var fullPath = config.FfmpegFolder.FullPath;
            return Path.Exists(fullPath)
                && directory.ContainsFile($"{config.FfprobeExeName}.exe")
                && directory.ContainsFile($"{config.FfmpegExeName}.exe");
        }



        public class Config(string folder = "ffmpeg", string ffmpegExeName = "ffmpeg", string ffprobeExeName = "ffprobe")
        {
            public const string Source = "https://github.com/GyanD/codexffmpeg/releases/download/2025-02-06-git-6da82b4485/ffmpeg-2025-02-06-git-6da82b4485-essentials_build.zip";

            public string Folder => folder;
            public string FfmpegExeName => ffmpegExeName;
            public string FfprobeExeName => ffprobeExeName;
            public IDirectory FfmpegFolder => new CwdDirectory(Folder);
        }
    }
}
