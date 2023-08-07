using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Web;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Container = YoutubeExplode.Videos.Streams.Container;
using System.Collections.ObjectModel;
using System.Windows;
using YoutubeDownloaderWpf.Controls;
using System.IO;
using System.Windows.Threading;

namespace YoutubeDownloaderWpf.Services
{
    public class YoutubeDownloader : IDownloader, INotifyPropertyChanged
    {
        private string _url = string.Empty;
        public string Url { get { return _url; } set { _url = value; OnPropertyChanged(); } }

        public ObservableCollection<DownloadStatus> DownloadStatuses { get; } = new();

        private static readonly YoutubeClient client = new();
        private string DDIR { get; set; } = Directory.GetCurrentDirectory();
        private static string DownloadFolderName { get; } = "Downloads";
        public string DownloadDirectoryPath => DownloadFolderName;
        public YoutubeDownloader()
           => Init();

        private static void Init()
            => Directory.CreateDirectory(DownloadFolderName);

        public Task Download()
            => Task.Factory.StartNew(() => DownloadAction(Url));


        private async void DownloadAction(string url)
        {
            await DispatchToUI(DownloadStatuses.Clear);
            bool isVideo;
            string[] urlSplit = url.Split('/');
            isVideo = urlSplit.Last().StartsWith("w");
            if (isVideo)
            {
                DownloadVideo(url, $"{DDIR}/{DownloadFolderName}");
            }
            else
            {
                DownloadPlaylist(url);
            }

        }

        private async void DownloadVideo(string url, string path)
        {
            var video = await client.Videos.GetAsync(url);
            string name = video.Title;
            var streamManifest = await client.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams().Where(s => s.Container == Container.Mp4).GetWithHighestBitrate();
            DownloadStatusContext statusContext = new(name.Split("/").Last(), streamInfo.Size.MegaBytes);
            await DispatchToUI(() => DownloadStatuses.Add(statusContext.AsStatus));
            var filePath = $"{path}/{name}.{streamInfo.Container}";
            var progressHandler = statusContext.ProgressHandler;
            ValueTask t = client.Videos.Streams.DownloadAsync(streamInfo, filePath, progressHandler);
            await t.AsTask().ContinueWith(t => statusContext.InvokeDownloadFinished(this, true));
            Trace.WriteLine($"Finished downloading from {url}");
        }

        private async void DownloadPlaylist(string url)
        {
            var playlist = await client.Playlists.GetAsync(url);
            Directory.CreateDirectory($"{DDIR}/{DownloadFolderName}/{playlist.Title}");
            await foreach (var batch in client.Playlists.GetVideoBatchesAsync(url))
            {
                foreach (var video in batch.Items)
                {
                    Trace.WriteLine($"Downloading from {video.Url}");
                    DownloadVideo(video.Url, $"{DDIR}/{DownloadFolderName}/{playlist.Title.Trim('/')}");
                    Trace.WriteLine($"Finished downloading from {url}");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
