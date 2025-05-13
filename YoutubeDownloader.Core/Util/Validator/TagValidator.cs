using System.Text.RegularExpressions;

namespace YoutubeDownloader.Core.Util.Validator;

public static partial class TagValidator
{
    [GeneratedRegex(@"^.*/v\d+\.\d+\.\d+$", RegexOptions.NonBacktracking)]
    private static partial Regex TagRegex();

    public static bool IsValid(string toTest)
        => TagRegex().IsMatch(toTest);
}
