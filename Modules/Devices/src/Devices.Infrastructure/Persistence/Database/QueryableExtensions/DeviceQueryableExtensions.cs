using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;

public static class DeviceQueryableExtensions
{
    public static IQueryable<Device> WithId(this IQueryable<Device> query, DeviceId id)
    {
        return query.Where(d => d.Id == id);
    }

    public static IQueryable<Device> OfIdentity(this IQueryable<Device> query, IdentityAddress address)
    {
        return query.Where(d => d.IdentityAddress == address);
    }

    public static IQueryable<Device> WithIdIn(this IQueryable<Device> query, IEnumerable<DeviceId> ids)
    {
        return query.Where(d => ids.Contains(d.Id));
    }
}
