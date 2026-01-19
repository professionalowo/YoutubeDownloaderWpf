using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public sealed class FfmpegDownloader(
    ILogger<FfmpegDownloader> logger,
    HttpClient client,
    FfmpegDownloader.Config config)
{
    public async ValueTask DownloadFfmpeg(CancellationToken token = default)
    {
        var zipBytes = await client.GetByteArrayAsync(Config.Source, token);
        await using var memoryStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(memoryStream);

        var ffmpegExeName = PlatformUtil.AsExecutablePath(config.FfmpegExeName);
        var entry = archive.Entries.FirstOrDefault(e =>
            e.FullName.EndsWith(ffmpegExeName, StringComparison.OrdinalIgnoreCase));
        if (entry == null)
        {
            logger.LogWarning("ffmpeg.exe not found in zip archive");
            throw new FileNotFoundException("ffmpeg.exe not found");
        }

        var destinationPath = config.Folder.ChildFileName(ffmpegExeName);
        Directory.CreateDirectory(config.Folder.FullPath);
        await using var entryStream = entry.Open();
        await using var targetFile = File.Create(destinationPath);
        await entryStream.CopyToAsync(targetFile, token);
        logger.LogInformation("ffmpeg.exe downloaded successfully");
    }

    public bool DoesFfmpegExist()
    {
        var directory = config.Folder;
        var fullPath = config.Folder.FullPath;
        return Path.Exists(fullPath)
               && directory.ContainsFile(PlatformUtil.AsExecutablePath(config.FfmpegExeName));
    }


    public sealed record Config(IDirectory Folder, string FfmpegExeName = Config.FfmpegName)
    {
        public const string FfmpegName = "ffmpeg";

        [StringSyntax(StringSyntaxAttribute.Uri)]
        public const string Source =
            "https://github.com/GyanD/codexffmpeg/releases/download/2025-02-06-git-6da82b4485/ffmpeg-2025-02-06-git-6da82b4485-essentials_build.zip";

        public static Config Default => new(new CwdDirectory(FfmpegName));
        public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
    }
}