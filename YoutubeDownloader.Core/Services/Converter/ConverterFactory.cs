using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public class ConverterFactory<TContext>(FfmpegDownloader.Config config)
    where TContext : IConverter<TContext>.IConverterContext
{
    private readonly Lazy<Mp3Converter<TContext>> _mp3Converter =
        new(() => new Mp3Converter<TContext>(config));

    private readonly Lazy<WriteThroughConverter<TContext>> _noopConverter =
        new(() => new WriteThroughConverter<TContext>(".mp4"));

    public IConverter<TContext> GetConverter(bool forceMp3) => forceMp3 switch
    {
        true => _mp3Converter.Value,
        false => _noopConverter.Value,
    };
}