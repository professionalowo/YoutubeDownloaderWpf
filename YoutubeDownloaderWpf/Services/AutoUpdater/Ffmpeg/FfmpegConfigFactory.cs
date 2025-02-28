using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Services.InternalDirectory;

namespace YoutubeDownloaderWpf.Services.AutoUpdater.Ffmpeg;
public class FfmpegConfigFactory
{
    public static FfmpegDownloader.Config ResolveConfig(IServiceProvider _)
    => GetConfigFromSystemPath(FfmpegDownloader.Config.FfmpegName) ?? FfmpegDownloader.Config.Default;

    private static FfmpegDownloader.Config? GetConfigFromSystemPath(string exe)
    {
        string replacedExe = Path.ChangeExtension(exe, "exe");
        string[] paths = [.. SplitPath(Environment.GetEnvironmentVariable("PATH")), .. SplitPath(Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User))];
        foreach (string path in paths.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)))
        {
            if (File.Exists(Path.Combine(path, replacedExe)))
            {
                return new(new AbsoluteDirectory(Path.GetFullPath(path)), exe);
            }
        }
        return null;
    }

    private static string[] SplitPath(string? variable) => (variable ?? "").Split(';');
}
