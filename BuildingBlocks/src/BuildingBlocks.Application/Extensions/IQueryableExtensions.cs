using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Backbone.BuildingBlocks.Application.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<TEntity> IncludeAll<TEntity>(
        this IQueryable<TEntity> query,
        DbContext context,
        int maxDepth = int.MaxValue) where TEntity : class
    {
        var includePaths = GetIncludePaths<TEntity>(context, maxDepth);

        foreach (var includePath in includePaths)
        {
            query = query.Include(includePath);
        }

        return query;
    }

    /// <remarks>
    ///     Adapted from https://stackoverflow.com/a/49597502/1636276
    /// </remarks>
    private static IEnumerable<string> GetIncludePaths<T>(DbContext context, int maxDepth = int.MaxValue)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth));

        var entityType = context.Model.FindEntityType(typeof(T)) ?? throw new Exception("Entity type not found in model");
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();

        while (true)
        {
            var entityNavigations = new List<INavigation>();

            if (stack.Count <= maxDepth)
                foreach (var navigation in entityType.GetNavigations())
                {
                    if (includedNavigations.Add(navigation))
                        entityNavigations.Add(navigation);
                }

            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }

                stack.Push(entityNavigations.GetEnumerator());
            }

            while (stack.Count > 0 && !stack.Peek().MoveNext())
            {
                stack.Pop();
            }

            if (stack.Count == 0)
                break;

            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}
