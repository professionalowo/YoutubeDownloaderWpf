using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class VideoDownloadService(YoutubeClient client, IDirectory downloads)
{
    public async Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(download.Url, token)
            .ConfigureAwait(false);
        var name = video.Title.ReplaceIllegalCharacters();
        return new NamedVideoDownload(download, name);
    }

    public string GetFileName(NamedVideoDownload named)
    {
        var (download, title) = named;
        var formatted = download.FormatName(title);
        if (download is PlaylistVideoDownload p)
        {
            downloads.CreateSubDirectory(p.PlaylistName);
        }

        return downloads.ChildFileName(formatted);
    }

    public async Task<Stream> GetStream(IStreamInfo info, CancellationToken token = default)
        => await client.Videos.Streams.GetAsync(info, token)
            .ConfigureAwait(false);


    public async Task<IStreamInfo> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(download.Url, token)
            .ConfigureAwait(false);
        return streamManifest.GetAudioStreams()
            .GetWithHighestBitrate();
    }
}