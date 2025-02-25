using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class Updater(ILogger<Updater> logger, HttpClient client, Updater.Version currentVersion)
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string LatestUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases/latest";
    public async Task<bool> IsNewVersionAvailable(CancellationToken token = default)
    {
        try
        {
            Version githubVersion = await GetNewestVersion(token);
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

    private async Task<Version> GetNewestVersion(CancellationToken token = default)
    {
        HttpResponseMessage response = await client.GetAsync(LatestUrl, token);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Request was not successfull");
        }
        string location = (response.RequestMessage?.RequestUri?.ToString()) ?? throw new HttpRequestException("Could not get newest tag");
        string tag = location.Split("/").Last().Trim().TrimStart('v');
        return Version.FromTag(tag);
    }

    public record Version(uint Major, uint Minor, uint Patch) : IComparable<Version>
    {
        public static Version NullVersion => new(0, 0, 0);
        public int CompareTo(Version? other)
        => other switch
        {
            null => 1,
            _ when Major != other.Major => Major.CompareTo(other.Major),
            _ when Minor != other.Minor => Minor.CompareTo(other.Minor),
            _ when Patch != other.Patch => Patch.CompareTo(other.Patch),
            _ => 0
        };

        public static Version FromTag(string tag)
        {
            if (tag.Split('.').Select(uint.Parse).Take(3).ToArray() is [uint major, uint minor, uint patch])
            {
                return new Version(major, minor, patch);
            }
            throw new ArgumentException("Tag did not conform to the pattern {major}.{minor}.{patch}");
        }
    }
}
