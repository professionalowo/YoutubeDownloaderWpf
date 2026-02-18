using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Core.Services.AutoUpdater.Ffmpeg;

public sealed record FfmpegConfig(IDirectory Folder, string FfmpegExeName = FfmpegConfig.FfmpegName)
{
    public const string FfmpegName = "ffmpeg";

    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string Source =
        "https://github.com/GyanD/codexffmpeg/releases/download/2026-02-15-git-33b215d155/ffmpeg-2026-02-15-git-33b215d155-essentials_build.7z";
    
    public string FfmpegExeFullPath => Folder.ChildFileName(FfmpegExeName);
}