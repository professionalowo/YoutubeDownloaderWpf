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
using System.Threading;
using System.CodeDom;
using Microsoft.Extensions.Logging;
using YoutubeDownloaderWpf.Util.Extensions;


namespace YoutubeDownloaderWpf.Services.Downloader
{
    public class YoutubeDownloader : IDownloader, INotifyPropertyChanged
    {
        private readonly ILogger<YoutubeDownloader> _logger;
        private string _url = string.Empty;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        private bool _forceMp3 = true;
        public bool ForceMp3
        {

            get => _forceMp3;
            set
            {
                _forceMp3 = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DownloadStatusContext> DownloadStatuses { get; } = [];
        public IEnumerable<CancellationTokenSource> CancellationSources => DownloadStatuses.Select(ds => ds.Cancellation);

        private string DDIR { get; set; } = Directory.GetCurrentDirectory();
        private static string DownloadFolderName { get; } = "Downloads";
        public string DownloadDirectoryPath => DownloadFolderName;

        private readonly YoutubeClient _client;

        private readonly Mp3Converter _mp3Converter;
        public YoutubeDownloader(ILogger<YoutubeDownloader> logger, YoutubeClient client, Mp3Converter converter)
        {
            _mp3Converter = converter;
            _client = client;
            _logger = logger;
            Init();
        }

        private static void Init()
            => Directory.CreateDirectory(DownloadFolderName);

        public async Task Download()
        => await DownloadAction(Url);



        private async Task DownloadAction(string url)
        {
            await DispatchToUI(DownloadStatuses.Clear);
            string[] urlSplit = url.Split('/');
            bool isVideo = urlSplit.Last().StartsWith('w');
            try
            {
                if (isVideo)
                {
                    await DownloadVideo(url, $"{DDIR}/{DownloadFolderName}");
                }
                else
                {
                    await DownloadPlaylist(url);
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async Task DownloadVideo(string url, string path)
        {
            var video = await _client.Videos.GetAsync(url);
            string name = video.Title.ReplaceIllegalCharacters();
            var streamManifest = await _client.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetAudioStreams().Where(s => s.Container == Container.Mp3 || s.Container == Container.Mp4).GetWithHighestBitrate();
            DownloadStatusContext statusContext = new(name.Split("/").Last(), streamInfo.Size.MegaBytes);
            DispatchToUISync(() => DownloadStatuses.Add(statusContext));
            var filePath = $"{path}/{name}.{streamInfo.Container}";
            if (File.Exists(filePath))
            {
                statusContext.InvokeDownloadFinished(this, true);
                return;
            }
            await _client.Videos.Streams.DownloadAsync(streamInfo, filePath, statusContext.ProgressHandler, statusContext.Cancellation.Token);
            if (ForceMp3)
            {
                await _mp3Converter.RunConversion(filePath, statusContext, default);
            }
            statusContext.InvokeDownloadFinished(this, true);

        }

        private async Task DownloadPlaylist(string url)
        {
            var playlist = await _client.Playlists.GetAsync(url);
            Directory.CreateDirectory($"{DDIR}/{DownloadFolderName}/{playlist.Title}");

            await foreach (var batch in _client.Playlists.GetVideoBatchesAsync(url))
            {
                Task.WaitAll(batch.Items.AsParallel().Select((video) => Task.Factory.StartNew(async () =>
                {
                    Trace.WriteLine($"Downloading from {video.Url}");
                    await DownloadVideo(video.Url, $"{DDIR}/{DownloadFolderName}/{playlist.Title.Trim('/')}");
                    Trace.WriteLine($"Finished downloading from {url}");
                })).ToArray());
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
        public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
