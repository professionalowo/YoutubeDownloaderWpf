using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Util
{
    public static class PathUtil
    {
        public static string ReplaceIllegalCharacters(string fileName, char replacement = '_') => string.Join(replacement, fileName.Split(Path.GetInvalidFileNameChars()));
    }
}
