namespace Backbone.Modules.Devices.Application.Extensions;
public static class IEnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> ienum, int batchSize = 100)
    {
        if (ienum == null)
        {
            throw new ArgumentNullException(nameof(ienum));
        }

        var list = ienum.ToList();

        if (!list.Any())
        {
            return new List<IEnumerable<T>>();
        }

        if (list.Count <= batchSize)
        {
            return new List<IEnumerable<T>>(new[] { list });
        }

        var response = new List<IEnumerable<T>>();

        for (var i = 0; i < list.Count; i += batchSize)
        {
            response.Add(
                new List<T>(
                    list.GetRange(i, Math.Min(batchSize, list.Count - i))
                )
            );
        }
        return response;
    }
}
