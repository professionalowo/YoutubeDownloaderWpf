using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Setup.Settings
{
    public class SettingsService(IRootDirectory root) : ISettingsService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
            { WriteIndented = true, TypeInfoResolver = AppConfigurationContext.Default };

        public const string settingsFileName = "settings.json";

        private string FullPath => Path.Combine(root.FullPath, settingsFileName);

        public ValueTask<AppConfiguration> LoadSettingsAsync(CancellationToken cancellationToken = default)
            => !File.Exists(FullPath)
                ? ValueTask.FromResult(new AppConfiguration())
                : LoadSettingsFileAsync(FullPath, cancellationToken);

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Handled in options")]
        private static async ValueTask<AppConfiguration> LoadSettingsFileAsync(string settingsFilePath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var settingsStream = File.OpenRead(settingsFilePath);
                return await JsonSerializer
                    .DeserializeAsync<AppConfiguration>(settingsStream, JsonOptions, cancellationToken)
                    .ConfigureAwait(false) ?? new AppConfiguration();
            }
            catch
            {
                return new AppConfiguration();
            }
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Handled in options")]
        public async Task SaveSettingsAsync(AppConfiguration settings, CancellationToken cancellationToken = default)
        {
            await using var settingsStream = File.Create(FullPath);
            await JsonSerializer.SerializeAsync(settingsStream, settings, JsonOptions, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}