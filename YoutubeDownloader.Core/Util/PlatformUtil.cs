using System.Runtime.InteropServices;

namespace YoutubeDownloader.Core.Util;

public class PlatformUtil
{
    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    
    
    public static string AsExecutablePath(string executablePath) => IsWindows() ? Path.ChangeExtension(executablePath,"exe") : executablePath;

    public static string GetExplorer()
    {
        if (IsWindows()) return "explorer.exe";
        return "open";
    }
}