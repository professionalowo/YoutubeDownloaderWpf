using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public class DownloadFactory<TContext>(YoutubeClient client, IDirectory downloads) where TContext: IConverter.IConverterContext
{
    public async IAsyncEnumerable<VideoDownload<TContext>> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var last = url.Split('/').Last().First();
        if (last == 'w')
        {
            yield return new VideoDownload<TContext>(client, url);
        }
        else
        {
            await foreach (var download in new PlaylistDownload<TContext>(client, url, downloads))
            {
                yield return download;
            }
        }
    }
}
