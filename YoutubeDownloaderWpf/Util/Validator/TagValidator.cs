using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Util.Validator;

public static partial class TagValidator
{
    [GeneratedRegex(@"^.*/v\d+\.\d+\.\d+$")]
    private static partial Regex TagRegex();

    public static bool IsValid(string toTest)
        => TagRegex().IsMatch(toTest);
}
