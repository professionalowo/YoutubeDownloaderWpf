using YoutubeDownloader.Core.Container;
using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class AudioConverter(string ffmpegPath, IMediaContainer target)
{
    public async Task Convert(Stream data, string outPath, IAudioConversionContext context,
        CancellationToken token = default)
    {
        var mp3Path = $"{outPath}.{target.Extension.Extension}";
        var metadata = new AudioMetadata(context.Name);
        await using var conversion = new FfmpegAudioConversion(ffmpegPath, mp3Path, target, metadata)
            .WithProgress(context.GetProgress());
        await data.CopyToAsync(conversion, token)
            .ConfigureAwait(false);
        context.InvokeDownloadFinished(this, true);
    }
}