using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Util.Extensions;

public static class PathExtensions
{
    public static string ReplaceIllegalCharacters(this string path, char replacement = '_') => PathUtil.ReplaceIllegalCharacters(path, replacement);
}
