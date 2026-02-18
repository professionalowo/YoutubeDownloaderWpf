using System.Text.RegularExpressions;

namespace YoutubeDownloader.Core.Services.AutoUpdater.GitHub.Validator;

public static partial class TagValidator
{
    [GeneratedRegex(@"^.*/v\d+\.\d+\.\d+$")]
    private static partial Regex TagRegex();

    public static bool IsValid(string toTest)
        => TagRegex().IsMatch(toTest);
}