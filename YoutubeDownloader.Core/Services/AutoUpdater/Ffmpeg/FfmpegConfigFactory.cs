using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public class FfmpegConfigFactory(FfmpegDownloader.Config defaultConfig)
{
    public FfmpegDownloader.Config ResolveConfig(IServiceProvider _)
        => GetConfigFromSystemPath(FfmpegDownloader.Config.FfmpegName) ?? defaultConfig;

    private static FfmpegDownloader.Config? GetConfigFromSystemPath(string exe)
    {
        var replacedExe = PlatformUtil.AsExecutablePath(exe);
        string[] paths =
        [
            .. SplitPath(Environment.GetEnvironmentVariable("PATH")),
            .. SplitPath(Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User))
        ];

        return paths.Where(p => !string.IsNullOrEmpty(p))
            .Where(p => File.Exists(Path.Combine(p, replacedExe)))
            .Select(Path.GetFullPath)
            .Select(p => new AbsoluteDirectory(p))
            .Select(dir => new FfmpegDownloader.Config(dir, exe))
            .FirstOrDefault();
    }

    private static string[] SplitPath(string? variable)
    {
        var toSplit = variable ?? "";
        var separator = PlatformUtil.IsWindows() ? ';' : ':';
        return toSplit.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}