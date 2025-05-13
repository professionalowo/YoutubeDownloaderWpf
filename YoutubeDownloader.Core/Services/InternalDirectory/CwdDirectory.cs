namespace YoutubeDownloader.Core.Services.InternalDirectory;

public class CwdDirectory(string name) : IDirectory
{
    public string FullPath { get; init; } = Path.Combine(Directory.GetCurrentDirectory(), name);
}
