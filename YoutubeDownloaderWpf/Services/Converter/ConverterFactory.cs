using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Services.AutoUpdater;

namespace YoutubeDownloaderWpf.Services.Converter
{
    public class ConverterFactory(FfmpegDownloader.Config config)
    {
        public IConverter GetGonverter(bool forceMp3) => forceMp3 switch
        {
            true => new Mp3Converter(config),
            false => new NoopConverter(".mp4"),
        };

    }
}
