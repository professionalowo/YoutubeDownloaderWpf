using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Services.InternalDirectory;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;
public class FfmpegConfigFactory
{
    public static FfmpegDownloader.Config ResolveConfig(IServiceProvider _)
    => GetConfigFromSystemPath(FfmpegDownloader.Config.FfmpegName) ?? FfmpegDownloader.Config.Default;



    private static FfmpegDownloader.Config? GetConfigFromSystemPath(string exe)
    {
        string[] paths = [.. SplitPath(Environment.GetEnvironmentVariable("PATH")), .. SplitPath(Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User))];
        foreach (string test in paths)
        {
            string path = test.Trim();
            if (!string.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, Path.ChangeExtension(exe,"exe"))))
            {
                return new(new AbsoluteDirectory(Path.GetFullPath(path)), exe);
            }
        }
        return null;
    }

    private static string[] SplitPath(string? variable) => (variable ?? "").Split(';');
}
