namespace Backbone.Modules.Devices.Application.Extensions;
public static class IEnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> items, int batchSize)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var list = items.ToList();

        if (!list.Any())
        {
            return new List<IEnumerable<T>>();
        }

        if (list.Count <= batchSize)
        {
            return new List<IEnumerable<T>>(new[] { list });
        }

        var batches = new List<IEnumerable<T>>();

        for (var i = 0; i < list.Count; i += batchSize)
        {
            batches.Add(
                new List<T>(
                    list.GetRange(i, Math.Min(batchSize, list.Count - i))
                )
            );
        }
        return batches;
    }
}
