using System.Diagnostics.CodeAnalysis;

namespace YoutubeDownloader.Core.Services.InternalDirectory;

public sealed class CwdDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string name) : IDirectory
{
    public string FullPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), name);
}