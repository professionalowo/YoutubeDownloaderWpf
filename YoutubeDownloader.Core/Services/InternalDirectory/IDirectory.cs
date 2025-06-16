using System.Collections.ObjectModel;
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

    string ChildFileName(params string[] fileNames) =>
        Path.Combine([FullPath, .. fileNames.Where(s => !string.IsNullOrEmpty(s))]);

    DirectoryInfo CreateSubDirectory(params string[] segments) =>
        Directory.CreateDirectory(Path.Combine([FullPath, .. segments.Where(s => !string.IsNullOrEmpty(s))]));

    Task<DirectoryInfo> CreateSubDirectoryAsync(params string[] segments) =>
        Task.Run(() => CreateSubDirectory(segments));

    bool ContainsFile(string name) => File.Exists(ChildFileName(name));

    string[] GetFiles(string searchPattern = "*") =>
        Directory.GetFiles(FullPath, searchPattern, SearchOption.AllDirectories);
}