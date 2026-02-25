using System.ComponentModel;
using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class ConverterFactory(FfmpegConfig config)
{
    public IConverter<TContext> GetConverter<TContext>(IMediaContainer kind)
        where TContext : IConverter<TContext>.IConverterContext => kind switch
    {
        MediaContainerKind.Mp3 => new Mp3Converter<TContext>(config.FfmpegExeFullPath),
        MediaContainerKind.Wav => new WriteThroughConverter<TContext>(".mp4"),
        _ => new WriteThroughConverter<TContext>(".mp4")
    };
}