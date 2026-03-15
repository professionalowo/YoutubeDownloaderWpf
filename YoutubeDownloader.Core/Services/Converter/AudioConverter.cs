using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class AudioConverter(string ffmpegPath, IMediaContainer target)
{
    public async Task Convert(Stream data, string outPath, IAudioConversionContext context, AudioMetadata metadata,
    CancellationToken token = default)
    {
        var audioPath = $"{outPath}.{target.Extension.Extension}";
        await using var conversion = new FfmpegAudioConversion(ffmpegPath, audioPath, target, metadata)
            .WithProgress(context.GetProgress());
        await data.CopyToAsync(conversion, token)
            .ConfigureAwait(false);
        context.InvokeDownloadFinished(this, true);
    }
}