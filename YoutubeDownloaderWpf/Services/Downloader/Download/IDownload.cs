using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public interface IDownload
    {
        public IEnumerable<Task<(string, DownloadStatusContext)>> ExecuteAsync(ObservableCollection<DownloadStatusContext> downloadStatuses);
    }
}
