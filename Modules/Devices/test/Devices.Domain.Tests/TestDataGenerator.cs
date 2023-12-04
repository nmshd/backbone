namespace Backbone.Modules.Devices.Domain.Tests;

public static class TestDataGenerator
{
    public static string GenerateString(int resultLength, char[]? chars = null)
    {
        chars ??= new char[] { 'A', 'B', 'C' };

        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
