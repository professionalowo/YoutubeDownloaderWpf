using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services.Downloader
{
    public interface IDownloader
    {
        string Url { get; }
        string DownloadDirectoryPath { get; }
        public IEnumerable<CancellationTokenSource> CancellationSources { get; }
        ObservableCollection<DownloadStatus> DownloadStatuses { get; }
        Task Download();
    }
}
