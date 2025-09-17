namespace YoutubeDownloader.Core.Services.InternalDirectory;

public sealed class ChildDirectory(IDirectory parent, string name) : IDirectory
{
    public string FullPath { get; } = Path.Combine(parent.FullPath, name);
}