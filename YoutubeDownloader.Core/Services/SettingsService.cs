using System.Text.Json;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.InternalDirectory;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services
{
    public class SettingsService(IRootDirectory root) : ISettingsService
    {
        private const string SettingsFileName = "settings.json";
        private AppConfiguration? _currentSettings;

        public AppConfiguration CurrentSettings => _currentSettings ??= LoadSettingsAsync().Result;

        public async Task<AppConfiguration> LoadSettingsAsync()
        {
            var settingsFilePath = Path.Combine(root.FullPath, SettingsFileName);
            if (!File.Exists(settingsFilePath))
            {
                return new AppConfiguration();
            }

            try
            {
                var json = await File.ReadAllTextAsync(settingsFilePath);
                return JsonSerializer.Deserialize<AppConfiguration>(json) ?? new AppConfiguration();
            }
            catch
            {
                return new AppConfiguration();
            }
        }

        public async Task SaveSettingsAsync(AppConfiguration settings)
        {
            var settingsFilePath = Path.Combine(root.FullPath, SettingsFileName);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(settingsFilePath, json);
            _currentSettings = settings;
        }
    }
}

