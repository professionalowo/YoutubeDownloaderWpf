using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public class Mp3Converter<TContext>(FfmpegDownloader.Config config)
    : IConverter<TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public ValueTask Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return ValueTask.FromCanceled(token);
        try
        {
            ConvertSync(audioStream, outPath, context);
            return ValueTask.CompletedTask;
        }
        catch (Exception e)
        {
            return ValueTask.FromException(e);
        }
    }

    private void ConvertSync(Stream data, ReadOnlySpan<char> outPath, TContext context)
    {
        var mp3Path = $"{outPath}.mp3";
        using var conversion = new FfmpegMp3Conversion(config.FfmpegExeFullPath, mp3Path);

        data.CopyToTracked(conversion.Input, context.GetProgress());
        context.InvokeDownloadFinished(this, true);
    }
}