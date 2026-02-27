using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class ConverterFactory(FfmpegConfig config)
{
    public AudioConverter GetConverter(IMediaContainer target) => new(config.FfmpegExeFullPath, target);
}