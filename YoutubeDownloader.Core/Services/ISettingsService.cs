using YoutubeDownloader.Core.Data;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services
{
    public interface ISettingsService
    {
        ValueTask<AppConfiguration> LoadSettingsAsync();
        Task SaveSettingsAsync(AppConfiguration settings);
    }
}

