using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;

public static class DeviceQueryableExtensions
{
    public static async Task<Device> FirstWithId(this IQueryable<Device> query, DeviceId id, CancellationToken cancellationToken)
    {
        var device = await query.WithId(id).FirstOrDefaultAsync(cancellationToken);

        return device ?? throw new NotFoundException(nameof(Device));
    }

    public static IQueryable<Device> WithId(this IQueryable<Device> query, DeviceId id)
    {
        return query.Where(d => d.Id == id);
    }

    public static IQueryable<Device> OfIdentity(this IQueryable<Device> query, IdentityAddress address)
    {
        return query.Where(d => d.IdentityAddress == address);
    }

    public static IQueryable<Device> NotDeleted(this IQueryable<Device> query)
    {
        return query.Where(Device.IsNotDeleted);
    }

    public static IQueryable<Device> WithIdIn(this IQueryable<Device> query, IEnumerable<DeviceId> ids)
    {
        return query.Where(d => ids.Contains(d.Id));
    }

    public static IQueryable<Device> IncludeUser(this IQueryable<Device> query)
    {
        return query.Include(d => d.User);
    }
}
