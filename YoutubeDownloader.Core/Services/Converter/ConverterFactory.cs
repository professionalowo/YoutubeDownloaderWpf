using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class ConverterFactory(FfmpegConfig config)
{
    public IConverter<TContext> GetConverter<TContext>(bool forceMp3)
        where TContext : IConverter<TContext>.IConverterContext => forceMp3 switch
    {
        true => new Mp3Converter<TContext>(config.FfmpegExeFullPath),
        false => new WriteThroughConverter<TContext>(".mp4"),
    };
}