namespace Backbone.Tooling.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> items)
    {
        return !items.Any();
    }
}
