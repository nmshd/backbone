namespace Backbone.Identity.Pool.Creator.Tools;
public static class IEnumeratorExtensionMethods
{
    public static T NextOrFirst<T>(this IEnumerator<T> enumerator)
    {
        if (enumerator.MoveNext()) return enumerator.Current;

        enumerator.Reset();
        enumerator.MoveNext();
        return enumerator.Current;
    }
}
