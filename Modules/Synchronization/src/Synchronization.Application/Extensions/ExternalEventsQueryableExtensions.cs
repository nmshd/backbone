using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class ExternalEventsQueryableExtensions
{
    public static IQueryable<ExternalEvent> WithOwner(this IQueryable<ExternalEvent> query, IdentityAddress owner)
    {
        return query.Where(d => d.Owner == owner);
    }

    public static IQueryable<ExternalEvent> WithErrorCountBelow(this IQueryable<ExternalEvent> query, byte maxErrorCount)
    {
        return query.Where(d => d.SyncErrorCount < maxErrorCount);
    }

    public static IQueryable<ExternalEvent> Unsynced(this IQueryable<ExternalEvent> query)
    {
        return query.Where(e => e.SyncRunId == null);
    }

    public static IQueryable<ExternalEvent> NotBlocked(this IQueryable<ExternalEvent> query)
    {
        return query.Where(e => !e.IsDeliveryBlocked);
    }

    public static IQueryable<ExternalEvent> Blocked(this IQueryable<ExternalEvent> query)
    {
        return query.Where(e => e.IsDeliveryBlocked);
    }

    public static IQueryable<ExternalEvent> WithType(this IQueryable<ExternalEvent> query, ExternalEventType type)
    {
        return query.Where(e => e.Type == type);
    }

    public static IQueryable<ExternalEvent> WithContext(this IQueryable<ExternalEvent> query, string context)
    {
        return query.Where(e => e.Context == context);
    }

    public static IQueryable<ExternalEvent> AssignedToSyncRun(this IQueryable<ExternalEvent> query, SyncRunId id)
    {
        return query.Where(i => i.SyncRunId == id);
    }
}
