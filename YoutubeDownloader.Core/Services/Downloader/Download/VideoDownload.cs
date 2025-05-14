using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public class VideoDownload<TContext>(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string url,
    string path = "") where TContext : IConverter<TContext>.IConverterContext
{
    public async ValueTask<DownloadData<StreamData, TContext>> GetStreamAsync(
        Func<string, double, TContext> contextFactory, CancellationToken token = default)
    {
        var nameTask = GetName(token);
        var streamInfo = await GetStreamInfo(token)
            .ConfigureAwait(false);
        var streamTask = client.Videos.Streams.GetAsync(streamInfo, token);

        var name = await nameTask
            .ConfigureAwait(false);
        var statusContext = contextFactory(name, streamInfo.Size.MegaBytes);
        var stream = await streamTask
            .ConfigureAwait(false);

        var data = new StreamData(stream, [path, name]);
        return new DownloadData<StreamData, TContext>(data, statusContext);
    }

    private async ValueTask<IStreamInfo> GetStreamInfo(CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url, token)
            .ConfigureAwait(false);
        var streamInfo = streamManifest.GetAudioStreams()
            .GetWithHighestBitrate();
        return streamInfo;
    }

    private async ValueTask<string> GetName(CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(url, token)
            .ConfigureAwait(false);
        return video.Title.ReplaceIllegalCharacters();
    }
}