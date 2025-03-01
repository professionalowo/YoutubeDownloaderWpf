using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YoutubeDownloaderWpf.Util;

public class UrlUtil
{
    private static string Combine(string first, string second)
        => $"{first.Trim('/')}/{second.Trim('/')}";

    public static string Combine(string first, params ReadOnlySpan<string> rest) => rest switch
    {
        [] => first,
        [var head] => Combine(first, head),
        [var head, .. var tail] => Combine(Combine(first, head), tail),
    };
}
