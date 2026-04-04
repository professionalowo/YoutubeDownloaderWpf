using Microsoft.Extensions.Options;
using YoutubeDownloader.Core.Data;

namespace YoutubeDownloader.Core.Services.InternalDirectory
{
    public class DownloadDirectoryFactory(IRootDirectory root, IOptionsMonitor<AppConfiguration> options)
        : IDownloadDirectoryFactory
    {
        public IDirectory Create()
        {
            var dir = root.ChildDirectory(options.CurrentValue.DownloadPath);
            dir.Init();
            return dir;
        }
    }
}

