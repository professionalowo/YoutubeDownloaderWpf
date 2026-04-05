using System.Text.Json.Serialization;

namespace YoutubeDownloader.Core.Data;

[JsonSerializable(typeof(AppConfiguration), GenerationMode = JsonSourceGenerationMode.Default)]
public partial class AppConfigurationContext : JsonSerializerContext
{
}