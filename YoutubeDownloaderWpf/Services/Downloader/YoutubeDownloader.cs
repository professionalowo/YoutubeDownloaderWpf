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
                var paths = await Task.WhenAll(downloadFactory.Get(url).ExecuteAsync(DownloadStatuses));
                if (ForceMp3)
                {
                    IEnumerable<Task> conversionTasks = paths.Select(
                            (context) => Task.Run(async () => await converter.RunConversion(context.Path, context.Context, default)));
                    await Task.WhenAll(conversionTasks);
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
                logger.LogError(e.ToString());
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
        public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
