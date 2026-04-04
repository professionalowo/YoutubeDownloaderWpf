using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace YoutubeDownloader.Setup;

public static class HttpClientExtension
{
    private static readonly HttpMessageHandler DefaultHandler = new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,
        EnableMultipleHttp2Connections = true,
        EnableMultipleHttp3Connections = true,

        AutomaticDecompression = DecompressionMethods.All,
        InitialHttp2StreamWindowSize = 1024 * 1024,

        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        ConnectTimeout = TimeSpan.FromSeconds(10),

        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests
    };

    extension(IHttpClientBuilder builder)
    {
        public IHttpClientBuilder UseDefaultHttpConfig()
            => builder
                .ConfigurePrimaryHttpMessageHandler(() => DefaultHandler)
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestVersion = HttpVersion.Version30;
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    client.Timeout = Timeout.InfiniteTimeSpan;
                });
    }
}