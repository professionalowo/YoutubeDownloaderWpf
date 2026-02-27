using System;
using System.Globalization;
using System.Windows.Controls;

namespace YoutubeDownloader.Wpf.Controls.Validation;
public class UriValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new(false, "url can't be empty");
            }
            if (!IsUriValid(url))
            {
                return new(false, "url is malformed");
            }
            return ValidationResult.ValidResult;
        }
        return new(false, "unreachable");
    }

    private static bool IsUriValid(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out var _);
}
