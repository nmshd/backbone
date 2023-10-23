using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Synchronization.Domain.Entities;

namespace Backbone.Synchronization.Application.Extensions;

public static class DatawalletModificationsQueryableExtensions
{
    public static IQueryable<DatawalletModification> CreatedBy(this IQueryable<DatawalletModification> query, IdentityAddress address)
    {
        return query.Where(e => e.CreatedBy == address);
    }
}
