using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public class ConverterFactory<T>(FfmpegDownloader.Config config) where T : IConverter<T>.IConverterContext
{
    private readonly Lazy<IConverter<T>> _mp3Converter = new(() => new Mp3Converter<T>(config));
    private readonly Lazy<IConverter<T>> _noopConverter = new(() => new WriteThroughConverter<T>(".mp4"));
    public IConverter<T> GetConverter(bool forceMp3) => forceMp3 switch
    {
        true => _mp3Converter.Value,
        false => _noopConverter.Value,
    };
}
