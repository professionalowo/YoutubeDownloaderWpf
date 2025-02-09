using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Util.ScopedResource;

namespace YoutubeDownloaderWpf.Services.AutoUpdater
{
    public class FfmpegDownloader(ILogger<FfmpegDownloader> logger, HttpClient client, FfmpegDownloader.Config config)
    {
        public async ValueTask<bool> DownloadFfmpeg()
        {

            if (DoesFfmpegExist(config)) return true;



            Directory.CreateDirectory(config.FfmpegFolder.FullPath);

            var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Downlaod Ffmpeg", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (res != MessageBoxResult.OK) return false;

            using var response = await client.GetAsync(Config.Source);
            using var readStream = await response.Content.ReadAsStreamAsync();

            using ScopedResource.File zipSource = new(config.FfmpegFolder.SaveFileName("source.zip"));
            using ScopedResource.Directory sourceUnzipped = new(config.FfmpegFolder.SaveFileName(Path.GetFileNameWithoutExtension(zipSource.FullPath)));

            using (FileStream fileStream = new(zipSource.FullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
            {
                readStream.CopyTo(fileStream);
            }


            ZipFile.ExtractToDirectory(zipSource.FullPath, sourceUnzipped.FullPath);
            string[] executables = Directory.GetFiles(sourceUnzipped.FullPath, "*.exe", SearchOption.AllDirectories);



            string ffmpegExe = AppendExe(config.FfmpegExeName);
            string ffprobeExe = AppendExe(config.FfprobeExeName);

            string ffmpegPathSource = executables.Where(path => path.EndsWith(ffmpegExe)).First();
            string ffprobePathSource = executables.Where(path => path.EndsWith(ffprobeExe)).First();

            File.Move(ffmpegPathSource, config.FfmpegFolder.SaveFileName(ffmpegExe));
            File.Move(ffprobePathSource, config.FfmpegFolder.SaveFileName(ffprobeExe));

            return true;
        }
        private static string AppendExe(string path) => path + ".exe";
        private static bool DoesFfmpegExist(Config config)
        {
            var directory = config.FfmpegFolder;
            var fullPath = config.FfmpegFolder.FullPath;
            return Path.Exists(fullPath)
                && directory.ContainsFile(AppendExe(config.FfprobeExeName))
                && directory.ContainsFile(AppendExe(config.FfmpegExeName));
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
