using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string @string)
        {
            return @string == string.Empty;
        }

        public static bool MatchesRegex(this string text, string regexString)
        {
            var regex = new Regex(regexString);
            return regex.IsMatch(text);
        }
    }
}