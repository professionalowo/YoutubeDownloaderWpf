using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class DownloadFactory(YoutubeClient client, IDirectory downloads)
{
    public async IAsyncEnumerable<VideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var last = url.Split('/').Last().First();
        if (last == 'w')
        {
            yield return new VideoDownload(client, url);
        }
        else
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
}