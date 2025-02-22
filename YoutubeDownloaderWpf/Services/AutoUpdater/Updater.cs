using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class Updater(ILogger<Updater> logger, HttpClient client)
{
    public ValueTask<bool> IsNewVersionAvailable()
    {
        return ValueTask.FromResult(false);
    }

    public Task UpdateVersion()
    {
        return new Task(() => { });
    }
}
