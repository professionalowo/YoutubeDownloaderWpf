using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class ConverterFactory<TContext>(FfmpegDownloader.Config config)
    where TContext : IConverter<TContext>.IConverterContext
{
    public IConverter<TContext> GetConverter(bool forceMp3) => forceMp3 switch
    {
        true => new Mp3Converter<TContext>(config.FfmpegExeFullPath),
        false => new WriteThroughConverter<TContext>(".mp4"),
    };
}