using AngleSharp.Io;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Util.Validator;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class GitHubVersionClient(HttpClient client)
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string LatestUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases/latest";
    public async Task<Updater.Version> GetNewestVersion(CancellationToken token = default)
    {
        using HttpResponseMessage response = await client.GetAsync(LatestUrl, token);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Request was not successfull");
        }
        string location = GetLocation(response) ?? throw new HttpRequestException("Could not get newest tag");
        if (!TagValidator.IsValid(location))
        {
            throw new ArgumentException($"Location {location} doesn't match the scheme v{{major}}.{{minor}}.{{patch}}");
        }
        string tag = GetTag(location);
        return Updater.Version.FromTag(tag);
    }

    private static string? GetLocation(HttpResponseMessage response)
        => response.RequestMessage?.RequestUri?.ToString();
    private static string GetTag(string location)
        => location.Split("/").Last().Trim().TrimStart('v');
}

