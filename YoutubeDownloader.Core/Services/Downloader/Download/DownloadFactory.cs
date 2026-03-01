using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class DownloadFactory(YoutubeClient client, IDirectory downloads)
{
    private static bool IsVideo([StringSyntax(StringSyntaxAttribute.Uri)] ReadOnlySpan<char> url)
    {
        var lastSlashIdx = url.LastIndexOf('/');

        if (lastSlashIdx == -1 || lastSlashIdx == url.Length - 1)
            throw new ArgumentException("Invalid url", nameof(url));

        var charAfterLastSlash = url[lastSlashIdx + 1];
        return charAfterLastSlash == 'w';
    }


    public IAsyncEnumerable<VideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default)
        => IsVideo(url)
            ? AsyncEnumerable.FromSingle(new VideoDownload(client, url), token)
            : GetPlaylist(url, token);


    private async IAsyncEnumerable<VideoDownload> GetPlaylist([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var playlist = await client.Playlists.GetAsync(url, token)
            .ConfigureAwait(false);
        var dir = await downloads.CreateSubDirectoryAsync(playlist.Title.ReplaceIllegalCharacters())
            .ConfigureAwait(false);
        var enumerable = client.Playlists.GetVideosAsync(url, token)
            .ConfigureAwait(false);
        await foreach (var video in enumerable)
        {
            yield return new VideoDownload(client, video.Url, dir.Name);
        }
    }
}