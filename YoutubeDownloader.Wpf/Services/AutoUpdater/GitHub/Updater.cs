using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Services.AutoUpdater.GitHub;

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
    public async ValueTask UpdateVersion(CancellationToken token = default)
    {
        TaggedVersion githubVersion = await client.GetNewestVersion(token);
        await client.DownloadVersion(githubVersion, token);
    }

    public class Noop(bool result) : IUpdater
    {
        public Noop() : this(false) { }

        ValueTask<bool> IUpdater.IsNewVersionAvailable(CancellationToken token)
            => ValueTask.FromResult(result);


        ValueTask IUpdater.UpdateVersion(CancellationToken token)
        => ValueTask.CompletedTask;
    }
}
