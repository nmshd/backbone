using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Backbone.Tooling.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty(this string @string)
    {
        return @string == string.Empty;
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? @string)
    {
        return string.IsNullOrEmpty(@string);
    }

    public static bool MatchesRegex(this string text, string regexString)
    {
        var regex = new Regex(regexString);
        return regex.IsMatch(text);
    }
}
