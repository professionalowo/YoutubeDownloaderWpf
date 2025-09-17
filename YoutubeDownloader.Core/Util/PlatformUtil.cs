namespace YoutubeDownloader.Core.Util;

public static class PlatformUtil
{
    public static bool IsWindows() => OperatingSystem.IsWindows();
    public static bool IsLinux() => OperatingSystem.IsLinux();
    public static bool IsMacOs() => OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();
    
    
    public static string AsExecutablePath(string executablePath) => IsWindows() ? Path.ChangeExtension(executablePath,"exe") : executablePath;

    public static string GetExplorer()
    {
        if (IsWindows()) return "explorer.exe";
        return "open";
    }
}