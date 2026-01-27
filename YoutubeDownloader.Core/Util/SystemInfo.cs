namespace YoutubeDownloader.Core.Util;

public static class SystemInfo
{
    /// <summary>
    /// Use 80% of cores
    /// </summary>
    public static int Cores => (int)Math.Ceiling(Environment.ProcessorCount * 0.5);
}