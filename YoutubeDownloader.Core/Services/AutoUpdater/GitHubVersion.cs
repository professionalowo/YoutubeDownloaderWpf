using System.Diagnostics.CodeAnalysis;

namespace YoutubeDownloader.Core.Services.AutoUpdater;

public class GitHubVersion
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string RepositoryUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf";
}