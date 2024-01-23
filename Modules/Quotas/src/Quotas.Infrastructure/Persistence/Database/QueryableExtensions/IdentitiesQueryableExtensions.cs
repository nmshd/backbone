using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
public static class IdentitiesQueryableExtensions
{
    public static async Task<IEnumerable<Identity>> WithTier(this IQueryable<Identity> query, string tierId, CancellationToken cancellationToken)
    {
        var identities = await query.Where(identity => identity.TierId == tierId).ToListAsync(cancellationToken);
        return identities;
    }

    public static async Task<Identity?> FirstWithAddress(this IQueryable<Identity> query, string address, CancellationToken cancellationToken)
    {
        return await query.Where(identity => identity.Address == address).FirstOrDefaultAsync(cancellationToken);
    }
}
