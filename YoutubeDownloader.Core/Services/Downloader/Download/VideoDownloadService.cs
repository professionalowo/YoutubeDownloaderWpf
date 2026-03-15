using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class VideoDownloadService(HttpClient http, YoutubeClient client, IDirectory downloads)
{
    public async Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(download.Url, token)
            .ConfigureAwait(false);

        var thumbnail = video.Thumbnails[^1].Url;
        var name = video.Title.ReplaceIllegalFileNameCharacters();
        var author = video.Author.ChannelTitle.ReplaceIllegalFileNameCharacters();
        return new NamedVideoDownload(download, name, author, thumbnail);
    }

    public string GetFileName(NamedVideoDownload named)
    {
        var (download, title, _, _) = named;
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

    public async Task<byte[]> GetThumbnail(NamedVideoDownload download, CancellationToken token = default)
        => await http.GetByteArrayAsync(download.ThumbnailUrl, token)
            .ConfigureAwait(false);
}