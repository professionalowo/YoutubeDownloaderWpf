using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Velopack;
using Velopack.Sources;
using YoutubeDownloader.Core.Services.AutoUpdater.GitHub;

namespace YoutubeDownloader.Wpf.Services;

public sealed class PackUpdater(ILogger<PackUpdater> logger)
{
    private readonly UpdateManager _manager = new UpdateManager(new GithubSource(GitHubVersionClient.url, null, false));

    public ValueTask CheckForAppUpdates(CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return ValueTask.FromCanceled(token);
        return !_manager.IsInstalled ? ValueTask.CompletedTask : UpdateAsync(token);
    }

    private async ValueTask UpdateAsync(CancellationToken token = default)
    {
        try
        {
            var newVersion = await _manager.CheckForUpdatesAsync();
            if (newVersion is null) return;

            await _manager.DownloadUpdatesAsync(newVersion, cancelToken: token);
            _manager.ApplyUpdatesAndRestart(newVersion);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check for updates.");
        }
    }
}