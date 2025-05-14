using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Extensions;

public static class PathExtensions
{
    public static string ReplaceIllegalCharacters(this string path, char replacement = '_') => PathUtil.ReplaceIllegalCharacters(path, replacement);
}
