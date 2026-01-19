using System.Threading.Channels;

namespace YoutubeDownloader.Core.Extensions;

public static class ChannelExtensions
{
    extension<T>(Channel<T> c)
    {
        public void Deconstruct(out ChannelReader<T> reader, out ChannelWriter<T> writer) =>
            (reader, writer) = (c.Reader, c.Writer);
    }
}