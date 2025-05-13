namespace YoutubeDownloader.Core.Services.AutoUpdater.GitHub;

public interface IUpdater
{
    public ValueTask<bool> IsNewVersionAvailable(CancellationToken token = default);
    public ValueTask UpdateVersion(string downloadDir,CancellationToken token = default);
}
