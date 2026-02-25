using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class AudioConverter<TContext>(string ffmpegPath, IMediaContainer target)
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
        var mp3Path = $"{outPath}.{target.Extension}";
        var metadata = new AudioMetadata(context.Name);
        await using var conversion = new FfmpegAudioConversion(ffmpegPath, mp3Path, target, metadata)
            .WithProgress(context.GetProgress());
        await data.CopyToAsync(conversion, token)
            .ConfigureAwait(false);
        context.InvokeDownloadFinished(this, true);
    }
}