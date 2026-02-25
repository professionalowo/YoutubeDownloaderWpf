using System.Diagnostics.CodeAnalysis;

namespace YoutubeDownloader.Core.Services.AutoUpdater;

public class GitHubVersion
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string url = "https://github.com/professionalowo/YoutubeDownloaderWpf";
}