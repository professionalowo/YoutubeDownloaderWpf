namespace YoutubeDownloader.Core.Services.InternalDirectory;

public sealed class CwdDirectory(string name) : IDirectory
{
    public string FullPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), name);
}