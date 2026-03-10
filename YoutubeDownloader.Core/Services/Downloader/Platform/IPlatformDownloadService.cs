using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Data.Download;

namespace YoutubeDownloader.Core.Services.Downloader.Platform;

public interface IPlatformDownloadService
{
    public IAsyncEnumerable<IVideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default);

    public Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default);

    public string GetFileName(NamedVideoDownload named);
    public Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default);


    public Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default);
}