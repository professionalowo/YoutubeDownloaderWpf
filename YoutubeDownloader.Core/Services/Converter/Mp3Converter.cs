using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Util.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public class Mp3Converter<T>(FfmpegDownloader.Config config) : IConverter<T> where T : IConverter<T>.IConverterContext
{
    public ValueTask<string> Convert(Stream data, string outPath, T context, CancellationToken token = default)
        => ValueTask.FromResult(ConvertSync(data, outPath, context));

    private string ConvertSync(Stream data, string outPath, T context)
    {
        var mp3Path = $"{outPath}.mp3";
        try
        {
            using FfmpegMp3Conversion conversion = new(config.FfmpegExeFullPath, mp3Path);
            data.CopyToTracked(conversion.Input, context.GetProgress());
            context.InvokeDownloadFinished(this, true);
            return mp3Path;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            File.Delete(mp3Path);
            throw;
        }
    }
}