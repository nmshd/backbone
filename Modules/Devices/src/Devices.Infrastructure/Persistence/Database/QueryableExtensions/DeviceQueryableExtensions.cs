using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;

public static class DeviceQueryableExtensions
{
    extension(IQueryable<Device> query)
    {
        public IQueryable<Device> WithId(DeviceId id)
        {
            return query.Where(d => d.Id == id);
        }

        public IQueryable<Device> OfIdentity(IdentityAddress address)
        {
            return query.Where(d => d.IdentityAddress == address);
        }

        public IQueryable<Device> WithIdIn(IEnumerable<DeviceId> ids)
        {
            return query.Where(d => ids.Contains(d.Id));
        }
    }
}
