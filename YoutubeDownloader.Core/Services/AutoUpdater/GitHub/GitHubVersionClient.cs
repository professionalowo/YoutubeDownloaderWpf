﻿using AngleSharp.Io;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Core.Util.Validator;

namespace YoutubeDownloader.Core.Services.AutoUpdater.GitHub;

public class GitHubVersionClient(HttpClient client)
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string ReleasesUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases";
    public async Task<TaggedVersion> GetNewestVersion(CancellationToken token = default)
    {
        string latestUrl = UrlUtil.Combine(ReleasesUrl, "latest");
        using HttpResponseMessage response = await client.GetAsync(latestUrl, token);
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
        return TaggedVersion.FromTag(tag);
    }

    public async Task DownloadVersion(String downloadDir,TaggedVersion version, CancellationToken token = default)
    {
        string zipFileName = $"YoutubeDownloader-{version}.zip";
        string fileUrl = UrlUtil.Combine(ReleasesUrl, "download", version.ToString(), zipFileName);

        using HttpResponseMessage response = await client.GetAsync(fileUrl, token);
        await using Stream readStream = await response.Content.ReadAsStreamAsync(token);

        string fullFilePath = Path.Combine(downloadDir, zipFileName);

        await using (FileStream outStream = new(fullFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
        {
            await readStream.CopyToAsync(outStream, token);
        }
    }

    private static string? GetLocation(HttpResponseMessage response)
        => response.RequestMessage?.RequestUri?.ToString();
    private static string GetTag(string location)
        => location.Split("/").Last().Trim().TrimStart('v');
}

