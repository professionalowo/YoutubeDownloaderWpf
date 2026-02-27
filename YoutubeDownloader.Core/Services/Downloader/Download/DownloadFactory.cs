using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
            var playlist = new PlaylistDownload(client, url, downloads)
                .WithCancellation(token)
                .ConfigureAwait(false);
            await foreach (var download in playlist)
            {
                yield return download;
            }
        }
    }
}