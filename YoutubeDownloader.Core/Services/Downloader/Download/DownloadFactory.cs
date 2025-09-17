using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public sealed class DownloadFactory<TContext>(YoutubeClient client, IDirectory downloads)
    where TContext : IConverter<TContext>.IConverterContext
{
    public async IAsyncEnumerable<VideoDownload<TContext>> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var last = url.Split('/').Last().First();
        if (last == 'w')
        {
            yield return new VideoDownload<TContext>(client, url);
        }
        else
        {
            var playlist = new PlaylistDownload<TContext>(client, url, downloads)
                .WithCancellation(token)
                .ConfigureAwait(false);
            await foreach (var download in playlist)
            {
                yield return download;
            }
        }
    }
}