using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YoutubeDownloaderWpf.Util;

public static class UrlUtil
{
    private  static string Combine(string first, string second)
        => $"{first.Trim('/')}/{second.Trim('/')}";

    /// <summary>
    /// Combines a variable amount of url segments
    /// </summary>
    /// <param name="first">The first segment</param>
    /// <param name="rest">A variable amount of other segments in 0...n</param>
    /// <returns>A combination of the segments separated by '/'</returns>
    public static string Combine(string first, params ReadOnlySpan<string> rest) => rest switch
    {
        [] => first,
        [var head] => Combine(first, head),
        [var head, .. var tail] => Combine(Combine(first, head), tail),
    };
}
