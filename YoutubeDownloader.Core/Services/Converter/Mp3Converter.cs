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

public class Mp3Converter<TContext>(FfmpegDownloader.Config config)
    : IConverter<TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public ValueTask<string> Convert(Stream data, string outPath, TContext context, CancellationToken token = default)
        => token.IsCancellationRequested
            ? ValueTask.FromCanceled<string>(token)
            : ValueTask.FromResult(ConvertSync(data, outPath, context));

    private string ConvertSync(Stream data, string outPath, TContext context)
    {
        var mp3Path = $"{outPath}.mp3";
        using var buffer = new BufferedStream(data);
        using var conversion = new FfmpegMp3Conversion(config.FfmpegExeFullPath, mp3Path);
        try
        {
            buffer.CopyToTracked(conversion.Input, context.GetProgress(), true);
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