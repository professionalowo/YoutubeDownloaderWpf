using System;
using System.Runtime.InteropServices;

namespace YoutubeDownloader.Wpf.Util;

public static class KnownFolders
{
    private static readonly Guid _downloadsGuid = new("374DE290-123F-4565-9164-39C4925E467B");
    public static string GetDownloadsPath() => SHGetKnownFolderPath(_downloadsGuid, 0);

    [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, nint hToken = 0);
}
