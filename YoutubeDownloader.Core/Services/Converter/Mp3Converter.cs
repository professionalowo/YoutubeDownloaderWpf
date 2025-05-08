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
    public ValueTask<string> Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return ValueTask.FromCanceled<string>(token);
        try
        {
            var mp3Path = ConvertSync(audioStream, outPath, context);
            return ValueTask.FromResult(mp3Path);
        }
        catch (Exception e)
        {
            return ValueTask.FromException<string>(e);
        }
    }

    private string ConvertSync(Stream data, ReadOnlySpan<char> outPath, TContext context)
    {
        var mp3Path = $"{outPath}.mp3";
        using var conversion = new FfmpegMp3Conversion(config.FfmpegExeFullPath, mp3Path);

        data.CopyToTracked(conversion.Input, context.GetProgress());
        context.InvokeDownloadFinished(this, true);

        return mp3Path;
    }
}