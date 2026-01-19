using System.Buffers;

namespace YoutubeDownloader.Core.Extensions;

public static class PathExtensions
{
    private static readonly SearchValues<char> IllegalChars = SearchValues.Create(Path.GetInvalidFileNameChars());

    extension(string path)
    {
        public string ReplaceIllegalCharacters(char replacement = '_')
        {
            if (path.IndexOfAny(IllegalChars) == -1)
            {
                return path;
            }

            return string.Create(path.Length, (path, replacement), static (dest, state) =>
            {
                var currentSource = state.path.AsSpan();
                var currentDest = dest;

                while (true)
                {
                    var index = currentSource.IndexOfAny(IllegalChars);

                    if (index == -1)
                    {
                        currentSource.CopyTo(currentDest);
                        break;
                    }

                    if (index > 0)
                    {
                        currentSource[..index].CopyTo(currentDest);
                    }

                    currentDest[index] = state.replacement;

                    currentSource = currentSource[(index + 1)..];
                    currentDest = currentDest[(index + 1)..];
                }
            });
        }
    }
}