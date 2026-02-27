using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Maui.Util;

public static partial class PlatformSpecificServiceExtensions
{
    public static partial IServiceCollection AddFfmpeg(this IServiceCollection serviceCollection,IDirectory directory);
}