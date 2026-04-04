namespace YoutubeDownloader.Core.Services.InternalDirectory;

public record RootDirectory(string FullPath) : AbsoluteDirectory(FullPath), IRootDirectory;


