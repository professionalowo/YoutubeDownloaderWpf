using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public class PlaylistDownload(
        YoutubeClient client,
        string url,
        IDirectory downloads) : IDownload
    {
        private async Task<IEnumerable<Task<(string, DownloadStatusContext)>>> ExecuteAsyncInternal(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {
            using var semaphore = new SemaphoreSlim(0, 1);
            var playlist = await client.Playlists.GetAsync(url);
            downloads.CreateSubDirectory(playlist.Title);
            List<PlaylistVideo> videos = [];
            List<Task<(string, DownloadStatusContext)>> pendingDownlaods = [];
            await foreach (var batch in client.Playlists.GetVideoBatchesAsync(url))
            {
                foreach (var video in batch.Items)
                {
                    videos.Add(video);
                }
            }
            foreach (var video in videos)
            {
                var download = new VideoDownload(client, video.Url, downloads).DownloadTo(downloadStatuses, downloads.SaveFileName(playlist.Title.Trim('/')));
                pendingDownlaods.Add(Task.Run(async () => await download));
            }
            return pendingDownlaods;
        }

        public IEnumerable<Task<(string, DownloadStatusContext)>> ExecuteAsync(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {
            var task = Task.Run(async () => await ExecuteAsyncInternal(downloadStatuses));
            task.Wait();

            return task.Result;
        }
    }
}
