using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Data.Download;

namespace YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;

public class YoutubePlatformService(YoutubeDownloadFactory factory, YoutubeVideoDownloadService downloader)
    : IPlatformDownloadService
{
    public IAsyncEnumerable<IVideoDownload> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url,
        CancellationToken token = default) => factory.Get(url, token);

    public async Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default) =>
        await downloader.GetName(download, token);

    public string GetFileName(NamedVideoDownload named)
        => downloader.GetFileName(named);

    public async Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default)
        => await downloader.GetStream(download, token);


    public async Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
        => await downloader.GetStreamInfo(download, token);
}