using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Wpf.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloader.Wpf.Services.Downloader.Download;

public class DownloadFactory(YoutubeClient client, IDirectory downloads)
{
    public async IAsyncEnumerable<VideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var last = url.Split('/').Last().First();
        if (last == 'w')
        {
            yield return new VideoDownload(client, url);
        }
        else
        {
            await foreach (var download in new PlaylistDownload(client, url, downloads))
            {
                yield return download;
            }
        }
    }
}
