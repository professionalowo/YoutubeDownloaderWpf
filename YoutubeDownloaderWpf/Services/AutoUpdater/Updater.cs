using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class Updater(ILogger<Updater> logger, GitHubVersionClient client, Updater.Version currentVersion)
{
    public async Task<bool> IsNewVersionAvailable(CancellationToken token = default)
    {
        try
        {
            Version githubVersion = await client.GetNewestVersion(token);
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
        return Task.CompletedTask;
    }

    public record Version(uint Major, uint Minor, uint Patch) : IComparable<Version>
    {
        public int CompareTo(Version? other)
        => other switch
        {
            null => 1,
            _ when Major != other.Major => Major.CompareTo(other.Major),
            _ when Minor != other.Minor => Minor.CompareTo(other.Minor),
            _ => Patch.CompareTo(other.Patch),
        };

        public static Version FromTag(string tag)
        {
            if (tag.Split('.').Select(uint.Parse).Take(3).ToArray() is [uint major, uint minor, uint patch])
            {
                return new(major, minor, patch);
            }
            throw new ArgumentException("Tag did not conform to the pattern {major}.{minor}.{patch}");
        }
    }
}
