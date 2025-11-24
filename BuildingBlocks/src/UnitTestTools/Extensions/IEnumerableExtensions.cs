namespace Backbone.UnitTestTools.Extensions;

public static class EnumerableExtensions
{
    public static T Second<T>(this IEnumerable<T> enumerable)
    {
        var asArray = enumerable.ToArray();
        if (asArray.Length < 2) throw new InvalidOperationException("Cannot get the second element because the enumerable contains less than 2 elements.");
        return asArray[1];
    }
}
