using System.Threading.Channels;

namespace YoutubeDownloader.Core.Extensions;

public static class ChannelExtensions
{
    extension<TWrite, TRead>(Channel<TWrite, TRead> c)
    {
        public void Deconstruct(out ChannelReader<TRead> reader, out ChannelWriter<TWrite> writer) =>
            (reader, writer) = (c.Reader, c.Writer);
    }
}