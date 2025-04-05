using Foundation;
using YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
using YoutubeDownloader.Core.Services.InternalDirectory;
namespace YoutubeDownloader.Maui.Util;

public static partial class PlatformSpecificServiceExtensions
{
    public static partial IServiceCollection AddFfmpeg(this IServiceCollection serviceCollection,IDirectory directory)
    {
        var config = new FfmpegDownloader.Config(directory,Path.Combine(NSBundle.MainBundle.ResourcePath, "ffmpeg"));
        serviceCollection.AddSingleton(config);
        
        return serviceCollection;
    }
}