using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Synchronization.Domain.Entities;

namespace Synchronization.Application.Extensions;

public static class DatawalletModificationsQueryableExtensions
{
    public static IQueryable<DatawalletModification> CreatedBy(this IQueryable<DatawalletModification> query, IdentityAddress address)
    {
        return query.Where(e => e.CreatedBy == address);
    }
}
