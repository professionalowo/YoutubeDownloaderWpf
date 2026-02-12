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
        await using var conversion =
            new FfmpegMp3Conversion(ffmpegPath, mp3Path, new Mp3Metadata(context.Name));
        await using var tracked = conversion.WithProgress(context.GetProgress());
        await data.CopyToAsync(tracked, token)
            .ConfigureAwait(false);
        context.InvokeDownloadFinished(this, true);
    }
}

public sealed record Mp3Metadata(string Name);