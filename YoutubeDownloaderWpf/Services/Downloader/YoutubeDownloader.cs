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
using YoutubeDownloaderWpf.Services.Converter;
<<<<<<< HEAD
<<<<<<< HEAD
using System.Threading;
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)

namespace YoutubeDownloaderWpf.Services.Downloader
{
    public class YoutubeDownloader : IDownloader, INotifyPropertyChanged
    {
        private string _url = string.Empty;
        public string Url { get { return _url; } set { _url = value; OnPropertyChanged(); } }

        private bool _forceMp3 = false;
        public bool ForceMp3
        {
<<<<<<< HEAD
<<<<<<< HEAD
            get => _forceMp3;
=======
            get => _forceMp3; 
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
            get => _forceMp3; 
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
            set
            {
                _forceMp3 = value;
                OnPropertyChanged();
            }
        }
<<<<<<< HEAD
<<<<<<< HEAD
        private Mp3Converter Mp3Converter { get; } = new();
=======
        private IConverter Mp3Converter { get; } = new Mp3Converter();
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
        private IConverter Mp3Converter { get; } = new Mp3Converter();
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
        public ObservableCollection<DownloadStatus> DownloadStatuses { get; } = new();

        public IEnumerable<CancellationTokenSource> CancellationSources => DownloadStatuses.Select(ds => ds.Context.Cancellation);

        private static readonly YoutubeClient client = new();
        private string DDIR { get; set; } = Directory.GetCurrentDirectory();
        private static string DownloadFolderName { get; } = "Downloads";
        public string DownloadDirectoryPath => DownloadFolderName;
        public YoutubeDownloader()
           => Init();

        private static void Init()
            => Directory.CreateDirectory(DownloadFolderName);

        public Task Download()
        {
            return Task.Factory.StartNew(() => DownloadAction(Url));
        }


        private async void DownloadAction(string url)
        {
            await DispatchToUI(DownloadStatuses.Clear);
            bool isVideo;
            string[] urlSplit = url.Split('/');
            isVideo = urlSplit.Last().StartsWith("w");
            try
            {
                if (isVideo)
                {
                    DownloadVideo(url, $"{DDIR}/{DownloadFolderName}");
                }
                else
                {
                    DownloadPlaylist(url);
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {

            }
            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException) { }
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
<<<<<<< HEAD
            if (File.Exists(filePath))
            {
                statusContext.InvokeDownloadFinished(this, true);
                return;
            }
            ValueTask t = client.Videos.Streams.DownloadAsync(streamInfo, filePath, progressHandler,statusContext.Cancellation.Token);
            Trace.WriteLine(ForceMp3);

            await t.AsTask().ContinueWith(t =>
            {
                if (ForceMp3)
                {
                    Mp3Converter.RunConversion(filePath, statusContext, statusContext.Cancellation.Token);
                }

            }, statusContext.Cancellation.Token).ContinueWith(t => statusContext.InvokeDownloadFinished(this, true), statusContext.Cancellation.Token);

=======
            ValueTask t = client.Videos.Streams.DownloadAsync(streamInfo, filePath, progressHandler);
            Trace.WriteLine(ForceMp3);
            await t.AsTask().ContinueWith(async(t) =>
            {
                
                if (ForceMp3)
                {
                    await Mp3Converter.RunConversion(filePath, statusContext);
                }
            }).ContinueWith(t => statusContext.InvokeDownloadFinished(this, true));
            Trace.WriteLine($"Finished downloading from {url}");
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
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
