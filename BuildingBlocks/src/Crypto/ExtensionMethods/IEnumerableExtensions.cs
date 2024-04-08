namespace Backbone.Crypto.ExtensionMethods;

public static class IEnumerableExtensions
{
    public static IEnumerable<TSource> TakeFrom<TSource>(this IEnumerable<TSource> source, int start)
    {
        return new ArraySegment<TSource>(source.ToArray(), start, source.Count() - start);
    }
}
