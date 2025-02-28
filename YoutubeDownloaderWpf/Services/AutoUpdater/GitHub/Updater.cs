using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater.GitHub;

public class Updater(ILogger<Updater> logger, GitHubVersionClient client, TaggedVersion currentVersion) : IUpdater
{
    public async ValueTask<bool> IsNewVersionAvailable(CancellationToken token = default)
    {
        try
        {
            TaggedVersion githubVersion = await client.GetNewestVersion(token);
            return currentVersion.CompareTo(githubVersion) < 0;
        }
        catch (Exception ex)
        {
            logger.LogError("{Error}", ex);
            return false;
        }
    }
    public ValueTask UpdateVersion(CancellationToken token = default)
    {
        return ValueTask.CompletedTask;
    }



    public class Noop : IUpdater
    {
        public ValueTask<bool> IsNewVersionAvailable(CancellationToken token = default)
            => ValueTask.FromResult(false);


        public ValueTask UpdateVersion(CancellationToken token = default)
        => ValueTask.CompletedTask;
    }
}
