using YoutubeDownloader.Core.Data;

namespace YoutubeDownloader.Setup.Settings
{
    public interface ISettingsService
    {
        ValueTask<AppConfiguration> LoadSettingsAsync(CancellationToken cancellationToken = default);
        Task SaveSettingsAsync(AppConfiguration settings, CancellationToken cancellationToken = default);
    }
}