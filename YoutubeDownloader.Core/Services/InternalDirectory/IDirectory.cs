using System.Diagnostics;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Services.InternalDirectory;

public interface IDirectory
{
    string FullPath { get; }

    void Init()
    {
        Directory.CreateDirectory(FullPath);
    }

    void Open()
    {
        Init();
        Process.Start(PlatformUtil.GetExplorer(), FullPath);
    }

    string ChildFileName(string fileName) =>
        Path.Join(FullPath, fileName);

    DirectoryInfo CreateSubDirectory(ReadOnlySpan<char> segment) =>
        Directory.CreateDirectory(Path.Join(FullPath, segment));

    ValueTask<DirectoryInfo> CreateSubDirectoryAsync(ReadOnlySpan<char> segment) =>
        ValueTask.FromResult(CreateSubDirectory(segment));

    bool ContainsFile(string name) => File.Exists(ChildFileName(name));

    IEnumerable<string> GetFiles(string searchPattern = "*") =>
        Directory.EnumerateFiles(FullPath, searchPattern, SearchOption.AllDirectories);

    IDirectory ChildDirectory(string name) => new ChildDirectory(this, name);
}