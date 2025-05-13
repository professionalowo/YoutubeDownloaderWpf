using System.Threading.Channels;

namespace YoutubeDownloader.Core.Util.Extensions;

public static class ChannelExtensions
{
    public static void Deconstruct<T>(this Channel<T> c, out ChannelReader<T> reader, out ChannelWriter<T> writer)
        => (reader, writer) = (c.Reader, c.Writer);
}