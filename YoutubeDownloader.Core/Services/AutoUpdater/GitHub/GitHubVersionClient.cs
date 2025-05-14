using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Core.Services.AutoUpdater.GitHub.Validator;

namespace YoutubeDownloader.Core.Services.AutoUpdater.GitHub;

public class GitHubVersionClient(HttpClient client)
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    private const string releasesUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases";

    public async Task<TaggedVersion> GetNewestVersion(CancellationToken token = default)
    {
        var latestUrl = UrlUtil.Combine(releasesUrl, "latest");
        using var response = await client.GetAsync(latestUrl, token);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Request was not successfull");
        }

        var location = GetLocation(response) ?? throw new HttpRequestException("Could not get newest tag");
        if (!TagValidator.IsValid(location))
        {
            throw new ArgumentException($"Location {location} doesn't match the scheme v{{major}}.{{minor}}.{{patch}}");
        }

        var tag = GetTag(location);
        return TaggedVersion.FromTag(tag);
    }

    public async Task DownloadVersion(string downloadDir, TaggedVersion version, CancellationToken token = default)
    {
        var zipFileName = $"YoutubeDownloader-{version}.zip";
        var fileUrl = UrlUtil.Combine(releasesUrl, "download", version.ToString(), zipFileName);

        using var response = await client.GetAsync(fileUrl, token);
        await using var readStream = await response.Content.ReadAsStreamAsync(token);

        var fullFilePath = Path.Combine(downloadDir, zipFileName);

        await using var outStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.ReadWrite,
            FileShare.ReadWrite | FileShare.Delete);
        await readStream.CopyToAsync(outStream, token);
    }

    private static string? GetLocation(HttpResponseMessage response)
        => response.RequestMessage?.RequestUri?.ToString();

    private static string GetTag(string location)
        => location.Split("/").Last().Trim().TrimStart('v');
}