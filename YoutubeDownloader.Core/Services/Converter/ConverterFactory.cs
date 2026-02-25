using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class ConverterFactory(FfmpegConfig config)
{
    public IConverter<TContext> GetConverter<TContext>(IMediaContainer target)
        where TContext : IConverter<TContext>.IConverterContext =>
        new AudioConverter<TContext>(config.FfmpegExeFullPath, target);
}