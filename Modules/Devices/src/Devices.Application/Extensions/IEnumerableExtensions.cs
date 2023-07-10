namespace Backbone.Modules.Devices.Application.Extensions;
public static class IEnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> ienum, int batchSize = 100)
    {
        if (ienum == null)
        {
            throw new ArgumentNullException(nameof(ienum));
        }

        if (!ienum.Any())
        {
            return new List<IEnumerable<T>>();
        }

        if (ienum.Count() <= batchSize)
        {
            return new List<IEnumerable<T>>(new[] { ienum });
        }

        var response = new List<IEnumerable<T>>();
        var list = ienum.ToList();
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
