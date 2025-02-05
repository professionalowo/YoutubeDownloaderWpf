using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater
{
    public class Updater(ILogger<Updater> logger,HttpClient client)
    {
        private readonly ILogger<Updater> _logger = logger;
        private readonly HttpClient _httpClient = client;

        public Task<bool> IsNewVersionAvailable()
        {
            return Task.FromResult(false);
        }

        public Task UpdateVersion() {
            return new Task(() => { });
        }
    }
}
