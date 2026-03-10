// ...existing code...

namespace YoutubeDownloader.Core.Data.Download;

/// <summary>
/// Abstraction for stream information provided by platform services.
/// Implementations should provide the size in megabytes and optionally
/// expose the underlying platform-specific info (e.g. a YoutubeExplode
/// IStreamInfo or a download URL for SoundCloud).
/// </summary>
public interface IPlatformStreamInfo
{
    double SizeInMb { get; }
    object Underlying { get; }
}