using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class PlaylistDownload(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string url,
    IDirectory downloads)
    : IAsyncEnumerable<VideoDownload>
{
    public async IAsyncEnumerator<VideoDownload> GetAsyncEnumerator(
        CancellationToken token = default)
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