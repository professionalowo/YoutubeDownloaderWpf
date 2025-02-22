using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Data;

public readonly record struct DownloadData<T>(T Data, DownloadStatusContext Context);

