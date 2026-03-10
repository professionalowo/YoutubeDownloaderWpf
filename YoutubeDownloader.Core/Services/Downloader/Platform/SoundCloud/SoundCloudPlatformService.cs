using SoundCloudExplode;
using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Core.Services.Downloader.Platform.SoundCloud;

public class SoundCloudPlatformService(SoundCloudClient client, IDirectory downloads) : IPlatformService
{
    public IAsyncEnumerable<IVideoDownload> Get(string url, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public string GetFileName(NamedVideoDownload named)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}