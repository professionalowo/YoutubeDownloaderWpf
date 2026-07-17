using System.Diagnostics.CodeAnalysis;

namespace YoutubeDownloader.Core.Services.AutoUpdater;

public static class GitHubVersion
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string RepositoryUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf";
}