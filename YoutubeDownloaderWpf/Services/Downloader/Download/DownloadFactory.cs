using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public class DownloadFactory(YoutubeClient client, IDirectory downloads)
    {
        public async IAsyncEnumerable<IDownload> Get(string url)
        {
            var last = url.Split('/').Last().First();
            if (last == 'w')
            {
                yield return new VideoDownload(client, url, downloads);
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
}
