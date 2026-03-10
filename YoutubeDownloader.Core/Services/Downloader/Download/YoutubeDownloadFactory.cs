using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class YoutubeDownloadFactory(YoutubeClient client, IDirectory downloads)
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
}