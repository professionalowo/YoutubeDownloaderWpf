﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Data;

namespace YoutubeDownloaderWpf.Services.Downloader.Download
{
    public interface IDownload
    {
        public string Path { get; }
        public Task<string> Name { get; }
        public Task<DownloadData<StreamData>> GetStreamAsync(ObservableCollection<DownloadStatusContext> downloadStatuses, CancellationToken token = default);
    }
}
