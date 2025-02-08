using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Data;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public interface IDownload
    {
        public IEnumerable<Task<DownloadData>> ExecuteAsync(ObservableCollection<DownloadStatusContext> downloadStatuses);
    }
}
