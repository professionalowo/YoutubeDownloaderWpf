using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class Mp3Converter<TContext>(string ffmpegPath)
    : IConverter<TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public ValueTask Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default)
        => token.IsCancellationRequested
            ? ValueTask.FromCanceled(token)
            : ConvertAsync(audioStream, outPath, context, token);


    private async ValueTask ConvertAsync(Stream data, string outPath, TContext context,
        CancellationToken token = default)
    {
        var mp3Path = $"{outPath}.mp3";
        await using var conversion = new FfmpegMp3Conversion<TContext>(ffmpegPath, mp3Path, context);
        await using var tracked = conversion.WithProgress(context.GetProgress());
        await data.CopyToAsync(tracked, token);
        context.InvokeDownloadFinished(this, true);
    }
}