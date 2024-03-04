using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class DatawalletModificationsQueryableExtensions
{
    public static IQueryable<DatawalletModification> CreatedBy(this IQueryable<DatawalletModification> query, IdentityAddress address)
    {
        return query.Where(e => e.CreatedBy == address);
    }
}
