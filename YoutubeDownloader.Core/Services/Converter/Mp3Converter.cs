using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

namespace YoutubeDownloader.Core.Services.Converter;

public class Mp3Converter<TContext>(string ffmpegPath)
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
        using var conversion = new FfmpegMp3Conversion(ffmpegPath, mp3Path);
        using var tracked = conversion.Input.Tracked(context.GetProgress());
        data.CopyTo(tracked);
        context.InvokeDownloadFinished(this, true);
    }
}