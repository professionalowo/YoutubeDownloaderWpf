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
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Services.Downloader.Download;
using System.Windows.Shapes;
using YoutubeDownloaderWpf.Data;


namespace YoutubeDownloaderWpf.Services.Downloader
{
    public class YoutubeDownloader(
        ILogger<YoutubeDownloader> logger,
        DownloadFactory downloadFactory,
        IConverter converter,
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

        private async Task DownloadAsMp3(string url)
        {
            
            List<Task<DownloadData<string>>> pathTasks = [];
            await foreach (var d in downloadFactory.Get(url))
            {
                var t = Task.Run(async()=> await d.ExecuteDownloadAsync(DownloadStatuses));
                pathTasks.Add(t);
            }
            var paths = await Task.WhenAll(pathTasks);
            IEnumerable<Task> conversionTasks = paths.Select(
                    (context) => Task.Run(async () => await converter.RunConversion(context.Data, context.Context, default)));
            await Task.WhenAll(conversionTasks);

        }

        private async Task _DownloadAsMp3(string url)
        {
            List<Task<DownloadData<(string, Stream)>>> tasks = [];

            await foreach (var d in downloadFactory.Get(url))
            {
                var t = Task.Run(async () => await d.GetStreamAsync(DownloadStatuses));
                tasks.Add(t);
            }

            var res = await Task.WhenAll(tasks);

            Trace.Write(res);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
        public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
