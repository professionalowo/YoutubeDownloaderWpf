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
        using var response = await client
            .GetAsync(new Uri(Config.Source), HttpCompletionOption.ResponseHeadersRead, token)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength ?? 0;

        await using var sourceStream = await response.Content.ReadAsStreamAsync(token)
            .ConfigureAwait(false);
        await using var memory = new MemoryStream((int)totalBytes)
            .WithProgress(new DownloadProgress(progress, totalBytes));

        await sourceStream.CopyToAsync(memory, token)
            .ConfigureAwait(false);

        //Stream has to be downloaded fully to be searchable
        await using var archive = await SevenZipArchive.OpenAsyncArchive(memory, cancellationToken: token)
            .ConfigureAwait(false);

        var ffmpegExeName = PlatformUtil.AsExecutablePath(config.FfmpegExeName);
        var entry = await archive.EntriesAsync.FirstOrDefaultAsync(e =>
                e.Key?.EndsWith(ffmpegExeName, StringComparison.OrdinalIgnoreCase) ?? false, token)
            .ConfigureAwait(false);
        if (entry == null)
        {
            logger.LogWarning("{Name} not found in zip archive", ffmpegExeName);
            throw new FileNotFoundException($"{ffmpegExeName} not found");
        }

        var destinationPath = config.Folder.ChildFileName(ffmpegExeName);
        Directory.CreateDirectory(config.Folder.FullPath);

        await using var entryStream = await entry.OpenEntryStreamAsync(token)
            .ConfigureAwait(false);
        await using var targetFile = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                81920,
                true
            )
            .WithProgress(new ExtractionProgress(progress, entry.Size));
        await entryStream.CopyToAsync(targetFile, token)
            .ConfigureAwait(false);
        logger.LogInformation("ffmpeg downloaded successfully");
    }

    public bool DoesFfmpegExist()
        => Path.Exists(config.Folder.FullPath)
           && config.Folder.ContainsFile(PlatformUtil.AsExecutablePath(config.FfmpegExeName));


    public sealed record Config(IDirectory Folder, string FfmpegExeName = Config.FfmpegName)
    {
        public const string FfmpegName = "ffmpeg";

        [StringSyntax(StringSyntaxAttribute.Uri)]
        public const string Source =
            "https://github.com/GyanD/codexffmpeg/releases/download/2026-02-15-git-33b215d155/ffmpeg-2026-02-15-git-33b215d155-essentials_build.7z";

        public static Config Default => new(new CwdDirectory(FfmpegName));
        public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
    }
}

internal class WeightedProgress(IProgress<double> parent, long total, double weight, double start) : IProgress<long>
{
    private long _reported;

    public void Report(long value)
    {
        _reported += value;
        var toReport = _reported / (double)total;
        parent.Report(toReport * weight + start);
    }
}

internal sealed class DownloadProgress(IProgress<double> parent, long total)
    : WeightedProgress(parent, total, 0.8, 0.0);

internal sealed class ExtractionProgress(IProgress<double> parent, long total)
    : WeightedProgress(parent, total, 0.2, 0.8);