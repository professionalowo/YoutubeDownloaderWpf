using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.InternalDirectory
{
    public interface IDirectory
    {
        string FullPath { get; }

        void Open() => Process.Start("explorer.exe", FullPath);

        string SaveFileName(params string[] fileNames) => Path.Combine([FullPath, .. fileNames.Where(s => !string.IsNullOrEmpty(s))]);
        DirectoryInfo CreateSubDirectory(params string[] segments) => Directory.CreateDirectory(Path.Combine([FullPath, .. segments.Where(s => !string.IsNullOrEmpty(s))]));

        Task<DirectoryInfo> CreateSubDirectoryAsync(params string[] segments) => Task.Run(() => CreateSubDirectory(segments));

        bool ContainsFile(string name) => new FileInfo(SaveFileName(name)).Exists;
    }
}
