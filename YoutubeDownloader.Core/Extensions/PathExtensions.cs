using System.Buffers;

namespace YoutubeDownloader.Core.Extensions;

public static class PathExtensions
{
    private static readonly SearchValues<char> IllegalChars = SearchValues.Create(Path.GetInvalidFileNameChars());

    extension(string path)
    {
        public string ReplaceIllegalFileNameCharacters(char replacement = '_')
        {
            var firstInvalidChar = path.IndexOfAny(IllegalChars);
            if (firstInvalidChar == -1)
            {
                return path;
            }

            return string.Create(path.Length, (path, firstInvalidChar, replacement), static (dest, state) =>
            {
                state.path.CopyTo(dest);
                var remainder = dest[state.firstInvalidChar..];

                for (var index = remainder.IndexOfAny(IllegalChars);
                     index >= 0;
                     index = remainder.IndexOfAny(IllegalChars))
                {
                    remainder[index] = state.replacement;

                    remainder = remainder[index..];
                }
            });
        }
    }
}