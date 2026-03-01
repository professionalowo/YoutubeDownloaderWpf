using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class VideoDownload(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string url,
    string path = "")
{
    public async Task<DownloadData> GetStreamAsync(
        Func<string, double, IAudioConversionContext> contextFactory, CancellationToken token = default)
    {
        var nameTask = GetName(token)
            .ConfigureAwait(false);
        var streamInfo = await GetStreamInfo(token)
            .ConfigureAwait(false);
        var streamTask = client.Videos.Streams.GetAsync(streamInfo, token)
            .ConfigureAwait(false);

        var name = await nameTask;
        var statusContext = contextFactory(name, streamInfo.Size.MegaBytes);
        var stream = await streamTask;

        var data = new StreamData(stream, [path, name]);
        return new DownloadData(data, statusContext);
    }

    private async Task<IStreamInfo> GetStreamInfo(CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url, token)
            .ConfigureAwait(false);
        return streamManifest.GetAudioStreams()
            .GetWithHighestBitrate();
    }

    private async Task<string> GetName(CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(url, token)
            .ConfigureAwait(false);
        return video.Title.ReplaceIllegalCharacters();
    }
}