using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Services.Downloader;

public interface IDownloader<TContext> where TContext : IConverter<TContext>.IConverterContext
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string Url { get; }
    ObservableCollection<TContext> DownloadStatuses { get; }
    Task Cancel();
    Task Download();
}
