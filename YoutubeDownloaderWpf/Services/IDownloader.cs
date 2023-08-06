using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services
{
    public interface IDownloader
    {
        string Url { get; }
        string DownloadDirectoryPath { get; }
        ObservableCollection<DownloadStatus> DownloadStatuses { get; }
        Task Download();
    }
}
