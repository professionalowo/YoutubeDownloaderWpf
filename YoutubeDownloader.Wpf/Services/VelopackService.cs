using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Velopack;
using Velopack.Locators;

namespace YoutubeDownloader.Wpf.Services;

public sealed class VelopackService(ILogger<VelopackService> logger, UpdateManager manager)
{
    public ValueTask CheckForAppUpdates(CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return ValueTask.FromCanceled(token);
        return !manager.IsInstalled ? ValueTask.CompletedTask : UpdateAsync(token);
    }

    private async ValueTask UpdateAsync(CancellationToken token = default)
    {
        try
        {
            var newVersion = await manager.CheckForUpdatesAsync();
            if (newVersion is null) return;

            await manager.DownloadUpdatesAsync(newVersion, cancelToken: token);
            manager.ApplyUpdatesAndRestart(newVersion);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check for updates.");
        }
    }
}

public static class UpdateManagerExtensions
{
    extension(UpdateManager manager)
    {
        public string? GetBasePath() => manager.IsInstalled ? VelopackLocator.Current.RootAppDir : null;
    }
}