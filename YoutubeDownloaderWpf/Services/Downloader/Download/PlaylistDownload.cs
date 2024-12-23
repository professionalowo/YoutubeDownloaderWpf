using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeExplode;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public class PlaylistDownload(
        YoutubeClient client,
        string url,
        IDirectory downloads) : IDownload
    {
        public async Task<IEnumerable<(string, DownloadStatusContext)>> Execute(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {
            var playlist = await client.Playlists.GetAsync(url);
            downloads.CreateSubDirectory(playlist.Title);
            ConcurrentQueue<Task<IEnumerable<(string, DownloadStatusContext)>>> pendingDownlaods = [];
            await foreach (var batch in client.Playlists.GetVideoBatchesAsync(url))
            {
                foreach (var video in batch.Items)
                {
                    var download = new VideoDownload(client, video.Url, downloads).DownloadTo(downloadStatuses, downloads.SaveFileName(playlist.Title.Trim('/')));
                    pendingDownlaods.Enqueue(download);
                }
            }


            return (await Task.WhenAll(pendingDownlaods)).SelectMany(i =>
            {
                return i.Select(downlaod => {
                    Trace.WriteLine($"Finished downloading {downlaod.Item2.Name}");
                    return downlaod;
                });
            });
        }
    }
}
