namespace Backbone.BuildingBlocks.Domain;

public static class StringUtils
{
    public static string Generate(char[] chars, int resultLength)
    {
        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
