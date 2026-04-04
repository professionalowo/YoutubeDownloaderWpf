using YoutubeDownloader.Core.Data;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services
{
    public interface ISettingsService
    {
        AppConfiguration CurrentSettings { get; }
        Task<AppConfiguration> LoadSettingsAsync();
        Task SaveSettingsAsync(AppConfiguration settings);
    }
}

