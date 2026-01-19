using YoutubeDownloader.Core.Util;

namespace YoutubeDownloader.Core.Extensions;

public static class PathExtensions
{
    extension(string path)
    {
        public string ReplaceIllegalCharacters(char replacement = '_') =>
            PathUtil.ReplaceIllegalCharacters(path, replacement);
    }
}