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

public class Mp3Converter(FfmpegDownloader.Config config) : IConverter
{
    public async ValueTask<string?> Convert(Stream data, string outPath, IConverter.IConverterContext context, CancellationToken token = default)
    {
        string mp3Path = $"{outPath}.mp3";
        try
        {
            await using FfmpegMp3Conversion conversion = new(config, mp3Path);
            await data.CopyToAsyncTracked(conversion.Input, context.GetProgress(), token);
            context.InvokeDownloadFinished(this, true);
            return mp3Path;
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            context.InvokeDownloadFinished(this, false);
            File.Delete(mp3Path);
        }
        catch (Exception)
        {
            File.Delete(mp3Path);
            throw;
        }
        return null;
    }
}
