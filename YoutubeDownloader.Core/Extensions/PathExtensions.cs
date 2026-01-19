using System.Buffers;

namespace YoutubeDownloader.Core.Extensions;

public static class PathExtensions
{
    private static readonly SearchValues<char> IllegalChars = SearchValues.Create(Path.GetInvalidFileNameChars());

    extension(string path)
    {
        public string ReplaceIllegalCharacters(char replacement = '_')
        {
            ReadOnlySpan<char> source = path;
            var firstInvalidChar = source.IndexOfAny(IllegalChars);
            if (firstInvalidChar == -1)
            {
                return path;
            }

            return string.Create(path.Length, (path, firstInvalidChar, replacement), static (dest, state) =>
            {
                state.path.AsSpan().CopyTo(dest);
                var remainder = dest[state.firstInvalidChar..];

                while (true)
                {
                    var index = remainder.IndexOfAny(IllegalChars);

                    if (index == -1) break;

                    remainder[index] = state.replacement;
                    
                    remainder = remainder[index..];
                }
            });
        }
    }
}