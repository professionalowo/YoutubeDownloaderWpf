using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class Updater(ILogger<Updater> logger, HttpClient client, Updater.Version currentVersion)
{
    public const string LatestUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases/latest";
    public async ValueTask<bool> IsNewVersionAvailable(CancellationToken token = default)
    {
        try
        {
            var response = await client.GetAsync(LatestUrl, token);
            string? location = response?.RequestMessage?.RequestUri?.ToString();
            if (location is null)
            {
                return false;
            }
            string tag = location.Split("/").Last().Trim().TrimStart('v');
            Version? githubVersion = Version.FromTag(tag);
            return currentVersion.CompareTo(githubVersion) < 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return false;
        }
    }

    public Task UpdateVersion()
    {
        return new Task(() => { });
    }

    public record Version(uint Major, uint Minor, uint Patch) : IComparable<Version>
    {
        public int CompareTo(Version? other)
        => other switch
        {
            null => 1,
            _ when Major != other.Major => Major.CompareTo(other.Major),
            _ when Minor != other.Minor => Minor.CompareTo(other.Minor),
            _ when Patch != other.Patch => Patch.CompareTo(other.Patch),
            _ => 0
        };

        public static Version? FromTag(string tag)
        {
            if (tag.Split('.').Select(uint.Parse).Take(3).ToArray() is [uint major, uint minor, uint patch])
            {
                return new Version(major, minor, patch);
            }
            return null;
        }
    }
}
