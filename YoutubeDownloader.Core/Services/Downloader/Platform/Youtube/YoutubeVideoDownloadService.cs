using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;

public sealed class YoutubeVideoDownloadService(YoutubeClient client, IDirectory downloads)
{
    public async Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(download.Url, token)
            .ConfigureAwait(false);
        var name = video.Title.ReplaceIllegalFileNameCharacters();
        return new NamedVideoDownload(download, name);
    }

    public string GetFileName(NamedVideoDownload named)
    {
        var (download, title) = named;
        var formatted = download.FormatName(title);
        return downloads.ChildFileName(formatted);
    }

    public async Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default)
        => await client.Videos.Streams.GetAsync(download.Info, token)
            .ConfigureAwait(false);


    public async Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(download.Url, token)
            .ConfigureAwait(false);
        var info = streamManifest.GetAudioStreams()
            .GetWithHighestBitrate();
        return new StreamVideoDownload(download, info);
    }
}