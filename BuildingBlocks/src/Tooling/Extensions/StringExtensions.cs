using System.Diagnostics.CodeAnalysis;
using System.Text;
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

    extension(string text)
    {
        public bool MatchesRegex(string regexString)
        {
            var regex = new Regex(regexString);
            return regex.IsMatch(text);
        }

        public string TruncateToXChars(int maxLength)
        {
            return text[..Math.Min(text.Length, maxLength)];
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
