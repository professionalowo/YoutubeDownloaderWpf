using YoutubeDownloader.Core.Data;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services
{
    public interface ISettingsService
    {
        ValueTask<AppConfiguration> LoadSettingsAsync(CancellationToken cancellationToken = default);
        Task SaveSettingsAsync(AppConfiguration settings, CancellationToken cancellationToken = default);
    }
}