namespace YoutubeDownloader.Core.Util.Extensions;

public static class PathExtensions
{
    public static string ReplaceIllegalCharacters(this string path, char replacement = '_') => PathUtil.ReplaceIllegalCharacters(path, replacement);
}
