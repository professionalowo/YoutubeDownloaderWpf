using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Util.ScopedResource;

namespace YoutubeDownloaderWpf.Services.AutoUpdater.Ffmpeg;

public class FfmpegDownloader(ILogger<FfmpegDownloader> logger, HttpClient client, FfmpegDownloader.Config config)
{
    public async ValueTask<bool> DownloadFfmpeg(CancellationToken token = default)
    {

        if (DoesFfmpegExist(config))
        {
            return true;
        }

        Directory.CreateDirectory(config.Folder.FullPath);

        var res = MessageBox.Show("YoutubeDowloader wants to download ffmpeg.\nContinue?", "Downlaod Ffmpeg", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        if (res != MessageBoxResult.OK)
        {
            return false;
        }

        using HttpResponseMessage response = await client.GetAsync(Config.Source, token);
        await using Stream readStream = await response.Content.ReadAsStreamAsync(token);

        using ScopedResource.File zipSource = new(config.Folder.ChildFileName("source.zip"));
        using ScopedResource.Directory sourceUnzipped = new(config.Folder.ChildFileName(Path.GetFileNameWithoutExtension(zipSource.FullPath)));

        await using (FileStream fileStream = new(zipSource.FullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
        {
            await readStream.CopyToAsync(fileStream, token);
        }


        ZipFile.ExtractToDirectory(zipSource.FullPath, sourceUnzipped.FullPath);
        string[] executables = Directory.GetFiles(sourceUnzipped.FullPath, "*.exe", SearchOption.AllDirectories);



        string ffmpegExe = Path.ChangeExtension(config.FfmpegExeName, "exe");
        string ffmpegPathSource = executables.Where(path => path.EndsWith(ffmpegExe)).First();
        File.Move(ffmpegPathSource, config.Folder.ChildFileName(ffmpegExe));

        logger.LogInformation("Installed Ffmpeg");
        return true;
    }
    private static bool DoesFfmpegExist(Config config)
    {
        var directory = config.Folder;
        var fullPath = config.Folder.FullPath;
        return Path.Exists(fullPath)
            && directory.ContainsFile(Path.ChangeExtension(config.FfmpegExeName, "exe"));
    }



    public record Config(IDirectory Folder, string FfmpegExeName = Config.FfmpegName)
    {
        public const string FfmpegName = "ffmpeg";

        [StringSyntax(StringSyntaxAttribute.Uri)]
        public const string Source = "https://github.com/GyanD/codexffmpeg/releases/download/2025-02-06-git-6da82b4485/ffmpeg-2025-02-06-git-6da82b4485-essentials_build.zip";
        public static Config Default => new(new CwdDirectory(FfmpegName));
        public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
    }
}

