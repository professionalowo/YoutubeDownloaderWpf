using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using YoutubeDownloaderWpf.Controls;
using System.IO;
using System.Windows.Threading;
using YoutubeDownloaderWpf.Services.Converter;
using System.Threading;
using System.CodeDom;
using Microsoft.Extensions.Logging;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Services.Downloader.Download;
using YoutubeDownloaderWpf.Data;
using YoutubeDownloaderWpf.Services.AutoUpdater;
using System.IO.Pipelines;
using YoutubeDownloaderWpf.Util.Extensions;


namespace YoutubeDownloaderWpf.Services.Downloader
{
    public class YoutubeDownloader(
        FfmpegDownloader.Config config,
        ILogger<YoutubeDownloader> logger,
        DownloadFactory downloadFactory,
        IDirectory downlaods) : IDownloader, INotifyPropertyChanged
    {

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

        public string DownloadDirectoryPath => downlaods.FullPath;


        public async Task Download()
        {
            await DispatchToUI(DownloadStatuses.Clear);
            await DownloadAction(Url);
        }



        private async Task DownloadAction(string url)
        {
            try
            {
                if (ForceMp3)
                    await DownloadAsMp3(url);
                else
                    await DownloadAsVideos(url);

            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }

        private async Task DownloadAsVideos(string url)
        {
            List<Task<DownloadData<string>>> pathTasks = [];
            await foreach (var d in downloadFactory.Get(url))
            {
                var t = Task.Run(async () => await d.ExecuteDownloadAsync(DownloadStatuses));
                pathTasks.Add(t);
            }
            await Task.WhenAll(pathTasks);
        }

        private async Task DownloadAsMp3(string url,CancellationToken token = default)
        {
            List<Task<DownloadData<(string[], Stream)>>> tasks = [];

            await foreach (var d in downloadFactory.Get(url))
            {
                var t = Task.Run(async () => await d.GetStreamAsync(DownloadStatuses));
                tasks.Add(t);
            }

            var res = await Task.WhenAll(tasks);
            List<Task> conversions = new(res.Length);
            SemaphoreSlim semaphoreSlim = new (5);
            foreach (var ((name,stream), context) in res) {
                await semaphoreSlim.WaitAsync(token);
                var task = Task.Run(async () =>
                {
                    var fileName = downlaods.SaveFileName(name);
                    await using FfmpegMp3Conversion conversion = new(config, fileName);
                    await stream.CopyToAsyncTracked(conversion.Input, GetProgressWrapper(context), token);
                    context.InvokeDownloadFinished(this, true);
                    semaphoreSlim?.Release();
                }, token);
            }
            await Task.WhenAll(conversions);
        }
        private static Progress<long> GetProgressWrapper(DownloadStatusContext context)
        {
            Progress<long> downloadProgress = new();
            downloadProgress.ProgressChanged += (_, e) => {
                var percentage = Math.Min(e / (context.Size * 1000), 100);
                if (context.ProgressHandler is IProgress<double> p)
                {
                    p.Report(percentage);
                }
            };

            return downloadProgress;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
        public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
