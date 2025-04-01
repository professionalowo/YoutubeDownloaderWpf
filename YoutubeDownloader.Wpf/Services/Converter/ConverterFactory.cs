using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Wpf.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Wpf.Services.Converter;

public class ConverterFactory(FfmpegDownloader.Config config)
{
    private readonly Lazy<IConverter> _mp3Converter = new(() => new Mp3Converter(config));
    private readonly Lazy<IConverter> _noopConverter = new(() => new WriteThroughConverter(".mp4"));
    public IConverter GetConverter(bool forceMp3) => forceMp3 switch
    {
        true => _mp3Converter.Value,
        false => _noopConverter.Value,
    };
}
