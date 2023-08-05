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
using System.Text.RegularExpressions;
using YoutubeDownloaderWpf.Controls;
using System.IO;

namespace YoutubeDownloaderWpf.Services
{
    public partial class YoutubeDownloader : IDownloader, INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _url = string.Empty;
        public string Url { get { return _url; } set { _url = value; OnPropertyChanged(); } }

        public ObservableCollection<DownloadStatus> DownloadStatuses { get; } = new();

        private static readonly YoutubeClient client = new();
        private string DDIR { get; set; } = System.IO.Directory.GetCurrentDirectory();
        private static string DownloadFolderName { get; } = "Downloads";
        public YoutubeDownloader()
        {
            Init();
        }

        private static void Init()
        {
            System.IO.Directory.CreateDirectory(DownloadFolderName);
        }
        public Task Download()
        {
            return Task.Factory.StartNew(() => DownloadAction(Url));
        }

        private void DownloadAction(string url)
        {
            DispatchToUI(DownloadStatuses.Clear);
            bool isVideo;
            string[] urlSplit = url.Split('/');
            isVideo = urlSplit.Last().StartsWith("w");
            Trace.WriteLine(isVideo);
            if (isVideo)
            {
                DownloadVideo(url, $"{DDIR}/{DownloadFolderName}", Name);
            }
            else
            {
                DownloadPlaylist(url);
            }

        }

        private async void DownloadVideo(string url, string path, string name)
        {
            name = PathRegex().Replace(name, string.Empty);
            if (string.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString();
            }
            var streamManifest = await client.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams().Where(s => s.Container == Container.Mp4).GetWithHighestBitrate();
            DownloadStatus? status = null;
            DispatchToUI(() =>
            {
                status = new(name.Split("/").Last(), streamInfo.Size.MegaBytes);
                DownloadStatuses.Add(status);
            });
            var filePath = $"{path}/{name}.{streamInfo.Container}";
            var progressHandler = new Progress<double>(p => status.Context.Progress = p*100);
            ValueTask t = client.Videos.Streams.DownloadAsync(streamInfo, filePath, progressHandler);

            await t.AsTask().ContinueWith(t =>
            {
                status?.Context.OnDownloadFinished(this, true);
            });
            Trace.WriteLine($"Finished downloading from {url}");
        }

        private async void DownloadPlaylist(string url)
        {
            var playlist = await client.Playlists.GetAsync(url);
            System.IO.Directory.CreateDirectory($"{DDIR}/{DownloadFolderName}/{playlist.Title}");
            await foreach (var batch in client.Playlists.GetVideoBatchesAsync(url))
            {
                foreach (var video in batch.Items)
                {
                    Trace.WriteLine($"Downloading from {video.Url}");
                    DownloadVideo(video.Url, $"{DDIR}/{DownloadFolderName}/{playlist.Title.Trim('/')}", $"{video.Title}");
                    Trace.WriteLine($"Finished downloading from {url}");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static void DispatchToUI(Action action)
        {
            Application.Current?.Dispatcher.Invoke(action);
        }

        [GeneratedRegex("[\\\\:\"*?<>|]+")]
        private static partial Regex PathRegex();
    }
}
