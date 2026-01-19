using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;
using SharpCompress.Archives.SevenZip;

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

        using var archive = SevenZipArchive.Open(memoryStream);
        
        var ffmpegExeName = PlatformUtil.AsExecutablePath(config.FfmpegExeName);
        var entry = archive.Entries.FirstOrDefault(e => e.Key?.EndsWith(ffmpegExeName, StringComparison.OrdinalIgnoreCase) ?? false);
        if (entry == null)
        {
            logger.LogWarning("{Name} not found in zip archive", ffmpegExeName);
            throw new FileNotFoundException($"{ffmpegExeName} not found");
        }

        var destinationPath = config.Folder.ChildFileName(ffmpegExeName);
        Directory.CreateDirectory(config.Folder.FullPath);
        await using var entryStream = await entry.OpenEntryStreamAsync(token);
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
            "https://github.com/GyanD/codexffmpeg/releases/download/2026-01-19-git-43dbc011fa/ffmpeg-2026-01-19-git-43dbc011fa-essentials_build.7z";

        public static Config Default => new(new CwdDirectory(FfmpegName));
        public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
    }
}