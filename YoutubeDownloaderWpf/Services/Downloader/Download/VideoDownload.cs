using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Data;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Util.Extensions;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public class VideoDownload(
        YoutubeClient client,
        string url,
        IDirectory downloads) : IDownload
    {
        public async Task<DownloadData> DownloadTo(ObservableCollection<DownloadStatusContext> downloadStatuses, string path = "")
        {
            var video = await client.Videos.GetAsync(url);
            string name = video.Title.ReplaceIllegalCharacters();
            var streamManifest = await client.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetAudioStreams().Where(s => s.Container == Container.Mp3 || s.Container == Container.Mp4).GetWithHighestBitrate();
            DownloadStatusContext statusContext = new(name.Split("/").Last(), streamInfo.Size.MegaBytes);
            await YoutubeDownloader.DispatchToUI(() => downloadStatuses.Add(statusContext));
            var filePath = downloads.SaveFileName(path, $"{name}.{streamInfo.Container}");
            if (!File.Exists(filePath))
            {
                await client.Videos.Streams.DownloadAsync(streamInfo, filePath, statusContext.ProgressHandler, statusContext.Cancellation.Token);
            }
            statusContext.InvokeDownloadFinished(this, true);
            return new(filePath, statusContext);

        }
        public IEnumerable<Task<DownloadData>> ExecuteAsync(ObservableCollection<DownloadStatusContext> downloadStatuses)
        {

            return [DownloadTo(downloadStatuses)];
        }
    }
}
