using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;

public static class IdentityQueryableExtensions
{
    public static async Task<Identity> FirstWithAddress(this IQueryable<Identity> query, IdentityAddress address, CancellationToken cancellationToken)
    {
        var identity = await query.FirstOrDefaultAsync(e => e.Address == address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        return identity;
    }

    public static async Task<Identity> FirstWithAddressOrDefault(this IQueryable<Identity> query, IdentityAddress address, CancellationToken cancellationToken)
    {
        var identity = await query.FirstOrDefaultAsync(e => e.Address == address, cancellationToken);

        return identity!;
    }
}
