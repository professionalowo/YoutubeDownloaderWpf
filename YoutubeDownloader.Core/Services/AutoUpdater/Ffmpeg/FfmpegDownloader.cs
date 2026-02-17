using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;
using SharpCompress.Archives.SevenZip;
using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public sealed class FfmpegDownloader(
    HttpClient client,
    ILogger<FfmpegDownloader> logger,
    FfmpegDownloader.Config config)
{
    public async ValueTask DownloadFfmpeg(IProgress<double> progress, CancellationToken token = default)
    {
        using var response = await client.GetAsync(Config.Source, HttpCompletionOption.ResponseHeadersRead, token)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength ?? 0;

        var downloadProgress = new Progress<long>(read => progress.Report(0.8 * (read / (double)totalBytes)));

        await using var sourceStream = await response.Content.ReadAsStreamAsync(token)
            .ConfigureAwait(false);
        await using var memory = new MemoryStream((int)totalBytes)
            .WithProgress(downloadProgress);

        await sourceStream.CopyToAsync(memory, token)
            .ConfigureAwait(false);

        //Stream has to be downloaded fully to be searchable
        using var archive = SevenZipArchive.OpenArchive(memory);

        var ffmpegExeName = PlatformUtil.AsExecutablePath(config.FfmpegExeName);
        var entry = archive.Entries.FirstOrDefault(e =>
            e.Key?.EndsWith(ffmpegExeName, StringComparison.OrdinalIgnoreCase) ?? false);
        if (entry == null)
        {
            logger.LogWarning("{Name} not found in zip archive", ffmpegExeName);
            throw new FileNotFoundException($"{ffmpegExeName} not found");
        }

        var destinationPath = config.Folder.ChildFileName(ffmpegExeName);
        Directory.CreateDirectory(config.Folder.FullPath);

        var extractProgress = new Progress<long>(written => progress.Report(0.8 + (double)written / entry.Size * 0.2));

        await using var entryStream = await entry.OpenEntryStreamAsync(token)
            .ConfigureAwait(false);
        await using var targetFile = File.Create(destinationPath)
            .WithProgress(extractProgress);
        await entryStream.CopyToAsync(targetFile, token)
            .ConfigureAwait(false);
        logger.LogInformation("ffmpeg.exe downloaded successfully");
    }

    public bool DoesFfmpegExist()
        => Path.Exists(config.Folder.FullPath)
           && config.Folder.ContainsFile(PlatformUtil.AsExecutablePath(config.FfmpegExeName));


    public sealed record Config(IDirectory Folder, string FfmpegExeName = Config.FfmpegName)
    {
        public const string FfmpegName = "ffmpeg";

        [StringSyntax(StringSyntaxAttribute.Uri)]
        public const string Source =
            "https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-essentials.7z";

        public static Config Default => new(new CwdDirectory(FfmpegName));
        public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
    }
}