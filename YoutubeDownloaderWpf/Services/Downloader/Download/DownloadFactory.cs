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
        public IDownload Get(string url)
        {
            string[] urlSplit = url.Split('/');
            return urlSplit.Last().First() switch
            {
                'w' => new VideoDownload(client, url, downloads),
                _ => new PlaylistDownload(client, url, downloads)
            };
        }
    }
}
