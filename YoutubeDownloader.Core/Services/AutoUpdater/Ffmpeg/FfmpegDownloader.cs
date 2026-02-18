using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Util;
using SharpCompress.Archives.SevenZip;
using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public sealed class FfmpegDownloader(
    HttpClient client,
    ILogger<FfmpegDownloader> logger,
    FfmpegConfig config)
{
    public async ValueTask DownloadFfmpeg(IProgress<double> progress, CancellationToken token = default)
    {
        await using var memory = await GetArchiveMemoryStream(progress, token);

        var ffmpegExeName = PlatformUtil.AsExecutablePath(config.FfmpegExeName);

        await using var archive = await SevenZipArchive.OpenAsyncArchive(memory, cancellationToken: token)
            .ConfigureAwait(false);
        var entry = await archive.EntriesAsync.FirstOrDefaultAsync(e =>
                e.Key?.EndsWith(ffmpegExeName, StringComparison.OrdinalIgnoreCase) ?? false, token)
            .ConfigureAwait(false);

        if (entry is null)
        {
            logger.LogError("ffmpeg not found in archive");
            throw new FileNotFoundException("ffmpeg not found in archive");
        }

        await using var entryStream = await entry.OpenEntryStreamAsync(token)
            .ConfigureAwait(false);

        await using var targetFile = CreateDestinationFile(ffmpegExeName)
            .WithProgress(new ExtractionProgress(progress, entry.Size));
        await entryStream.CopyToAsync(targetFile, token)
            .ConfigureAwait(false);

        logger.LogInformation("ffmpeg downloaded successfully");
    }

    private async Task<Stream> GetArchiveMemoryStream(IProgress<double> progress,
        CancellationToken token = default)
    {
        using var response = await client
            .GetAsync(new Uri(FfmpegConfig.Source), HttpCompletionOption.ResponseHeadersRead, token)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength ?? 0;

        await using var sourceStream = await response.Content.ReadAsStreamAsync(token)
            .ConfigureAwait(false);
        var memory = new MemoryStream((int)totalBytes)
            .WithProgress(new DownloadProgress(progress, totalBytes));

        await sourceStream.CopyToAsync(memory, token)
            .ConfigureAwait(false);

        return memory;
    }

    private FileStream CreateDestinationFile(string ffmpegExeName)
    {
        var destinationPath = config.Folder.ChildFileName(ffmpegExeName);
        config.Folder.Init();

        return new FileStream(
            destinationPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            true
        );
    }

    public bool DoesFfmpegExist()
        => Path.Exists(config.Folder.FullPath)
           && config.Folder.ContainsFile(PlatformUtil.AsExecutablePath(config.FfmpegExeName));
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