using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Wpf.Controls;

namespace YoutubeDownloader.Wpf.Services.Downloader;

public interface IDownloader
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string Url { get; }
    ObservableCollection<DownloadStatusContext> DownloadStatuses { get; }
    Task Cancel();
    Task Download();
}
