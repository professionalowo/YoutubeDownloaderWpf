using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;
public class FfmpegConfigFactory(FfmpegDownloader.Config defaultConfig)
{
    public FfmpegDownloader.Config ResolveConfig(IServiceProvider _)
    => GetConfigFromSystemPath(FfmpegDownloader.Config.FfmpegName) ?? defaultConfig;

    private static FfmpegDownloader.Config? GetConfigFromSystemPath(string exe)
    {
        string replacedExe = PlatformUtil.AsExecutablePath(exe);
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

    private static string[] SplitPath(string? variable)
    {
        string toSplit = variable ?? ""; 
        char separator = PlatformUtil.IsWindows() ? ';' : ':';
        return toSplit.Split(separator);
    }
}
