﻿using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public class FfmpegDownloader(ILogger<FfmpegDownloader> logger, HttpClient client, FfmpegDownloader.Config config)
{
    public async ValueTask DownloadFfmpeg(CancellationToken token = default)
    {
        Directory.CreateDirectory(config.Folder.FullPath);
        using var response = await client.GetAsync(Config.Source, token);
        await using var readStream = await response.Content.ReadAsStreamAsync(token);

        using ScopedResource.File zipSource = new(config.Folder.ChildFileName("source.zip"));
        using ScopedResource.Directory sourceUnzipped = new(config.Folder.ChildFileName(Path.GetFileNameWithoutExtension(zipSource.FullPath)));

        await using (var fileStream = new FileStream(zipSource.FullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
        {
            await readStream.CopyToAsync(fileStream, token);
        }

        ZipFile.ExtractToDirectory(zipSource.FullPath, sourceUnzipped.FullPath);
        var executables = Directory.GetFiles(sourceUnzipped.FullPath, "*.exe", SearchOption.AllDirectories);

        var ffmpegExe = Path.ChangeExtension(config.FfmpegExeName, "exe");
        var ffmpegPathSource = executables.First(path => path.EndsWith(ffmpegExe));
        File.Move(ffmpegPathSource, config.Folder.ChildFileName(ffmpegExe));

        logger.LogInformation("Installed Ffmpeg");
    }

    public bool DoesFfmpegExist()
    {
        var directory = config.Folder;
        var fullPath = config.Folder.FullPath;
        return Path.Exists(fullPath)
            && directory.ContainsFile(PlatformUtil.AsExecutablePath(config.FfmpegExeName));
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

