using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
public static class IdentitiesQueryableExtensions
{
    public static IEnumerable<Identity> WithTier(this IQueryable<Identity> query, string tierId)
    {
        var identities = query.Where(identity => identity.TierId == tierId);
        return identities;
    }
}
