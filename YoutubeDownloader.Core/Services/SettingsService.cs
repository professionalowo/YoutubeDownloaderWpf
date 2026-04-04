using System.Text.Json;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Core.Services
{
    public class SettingsService(IRootDirectory root) : ISettingsService
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private const string SettingsFileName = "settings.json";

        private string FullPath => Path.Combine(root.FullPath, SettingsFileName);

        public ValueTask<AppConfiguration> LoadSettingsAsync()
            => !File.Exists(FullPath)
                ? ValueTask.FromResult(new AppConfiguration())
                : LoadSettingsFileAsync(FullPath);


        private static async ValueTask<AppConfiguration> LoadSettingsFileAsync(string settingsFilePath)
        {
            try
            {
                await using var settingsStream = File.OpenRead(settingsFilePath);
                return await JsonSerializer.DeserializeAsync<AppConfiguration>(settingsStream, JsonOptions) ??
                       new AppConfiguration();
            }
            catch
            {
                return new AppConfiguration();
            }
        }

        public async Task SaveSettingsAsync(AppConfiguration settings)
        {
            await using var settingsStream = File.OpenRead(FullPath);
            await JsonSerializer.SerializeAsync(settingsStream, JsonOptions);
        }
    }
}