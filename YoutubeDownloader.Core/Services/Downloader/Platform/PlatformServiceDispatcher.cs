using YoutubeDownloader.Core.Services.Downloader.Platform.SoundCloud;
using YoutubeDownloader.Core.Services.Downloader.Platform.Youtube;

namespace YoutubeDownloader.Core.Services.Downloader.Platform;

public class PlatformServiceDispatcher(YoutubePlatformService youtube, SoundCloudPlatformService soundCloud)
{
    public IPlatformService GetServiceForUrl(ReadOnlySpan<char> url)
        => url.StartsWith("https://www.youtube.com") ? youtube : soundCloud;
}