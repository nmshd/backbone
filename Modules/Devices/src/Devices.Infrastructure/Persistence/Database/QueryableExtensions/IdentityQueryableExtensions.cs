using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;

public static class IdentityQueryableExtensions
{
    public static async Task<Identity?> FirstWithAddressOrDefault(this IQueryable<Identity> query, IdentityAddress address, CancellationToken cancellationToken)
    {
        var identity = await query.FirstOrDefaultAsync(e => e.Address == address, cancellationToken);

        return identity;
    }
}
