using Backbone.Modules.Quotas.Domain.Aggregates;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
public static class CreatedAtQueryableExtensions
{
    public static IQueryable<T> CreatedInInterval<T>(this IQueryable<T> query, DateTime from, DateTime to) where T : ICreatedAt
    {
        return query.Where(it => it.CreatedAt >= from && it.CreatedAt <= to);
    }
}
