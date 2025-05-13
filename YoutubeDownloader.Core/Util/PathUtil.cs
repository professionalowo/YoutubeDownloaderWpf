namespace YoutubeDownloader.Core.Util;

public static class PathUtil
{
    public static string ReplaceIllegalCharacters(string fileName, char replacement = '_') => string.Join(replacement, fileName.Split(Path.GetInvalidFileNameChars()));
}
