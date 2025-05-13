using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
