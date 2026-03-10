using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;

public class YoutubePlatformService(YoutubeClient client, IDirectory downloads)
    : IPlatformService
{
    private static bool IsVideo([StringSyntax(StringSyntaxAttribute.Uri)] ReadOnlySpan<char> url)
    {
        var lastSlashIdx = url.LastIndexOf('/');

        if (lastSlashIdx == -1 || lastSlashIdx == url.Length - 1)
            throw new ArgumentException("Invalid url", nameof(url));

        var charAfterLastSlash = url[lastSlashIdx + 1];
        return charAfterLastSlash == 'w';
    }


    public IAsyncEnumerable<IVideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default)
        => IsVideo(url)
            ? AsyncEnumerable.FromSingle(new SingleVideoDownload(url), token)
            : GetPlaylist(url, token);


    private async IAsyncEnumerable<PlaylistVideoDownload> GetPlaylist(
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string url,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var enumerable = client.Playlists.GetVideosAsync(url, token)
            .ConfigureAwait(false);
        var playlist = await client.Playlists.GetAsync(url, token)
            .ConfigureAwait(false);
        var title = playlist.Title.ReplaceIllegalFileNameCharacters();
        await downloads.CreateSubDirectoryAsync(title);
        await foreach (var video in enumerable)
        {
            yield return new PlaylistVideoDownload(title, video.Url);
        }
    }

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

    private sealed record YoutubePlatformStreamInfo(IStreamInfo Info) : IPlatformStreamInfo
    {
        public double SizeInMb => Info.Size.MegaBytes;
        public object Underlying => Info;
    }

    public async Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default)
    {
        if (download.Info.Underlying is IStreamInfo ytInfo)
            return await client.Videos.Streams.GetAsync(ytInfo, token)
                .ConfigureAwait(false);

        throw new ArgumentException("Invalid stream info", nameof(download));
    }

    public async Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(download.Url, token)
            .ConfigureAwait(false);
        var info = streamManifest.GetAudioStreams()
            .GetWithHighestBitrate();
        return new StreamVideoDownload(download, new YoutubePlatformStreamInfo(info));
    }
}