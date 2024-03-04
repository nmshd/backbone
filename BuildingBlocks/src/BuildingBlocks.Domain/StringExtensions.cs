namespace Backbone.BuildingBlocks.Domain;

public static class StringExtensions
{
    public static bool ContainsOnly(this string str, IEnumerable<char> chars)
    {
        var enumerable = chars as char[] ?? chars.ToArray();
        var stringChars = str.ToCharArray();

        if (stringChars.Any(stringChar => enumerable.All(validChar => validChar != stringChar))) return false;

        return true;
    }
}
