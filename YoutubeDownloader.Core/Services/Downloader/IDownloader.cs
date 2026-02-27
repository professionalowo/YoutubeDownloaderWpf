using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Services.Downloader;

public interface IDownloader
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string Url { get; }

    ObservableCollection<IAudioConversionContext> DownloadStatuses { get; }
    Task Cancel();
    Task Download();
}