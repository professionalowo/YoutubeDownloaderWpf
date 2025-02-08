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
using YoutubeDownloaderWpf.Data;
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
        private async ValueTask<IEnumerable<Task<DownloadData>>> ExecuteAsyncInternal(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {
            var playlist = await client.Playlists.GetAsync(url);
            await downloads.CreateSubDirectoryAsync(playlist.Title);
            List<Task<DownloadData>> pendingDownlaods = [];
            await foreach (var video in client.Playlists.GetVideosAsync(url))
            {
                var download = new VideoDownload(client, video.Url, downloads).DownloadTo(downloadStatuses, downloads.SaveFileName(playlist.Title.Trim('/')));
                pendingDownlaods.Add(Task.Run(async () => await download));
            }
            return pendingDownlaods;
        }

        public IEnumerable<Task<DownloadData>> ExecuteAsync(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {
            var task = Task.Run(async () => await ExecuteAsyncInternal(downloadStatuses));
            task.Wait();

            return task.Result;
        }
    }
}
