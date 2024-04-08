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

    public static IQueryable<ExternalEvent> AssignedToSyncRun(this IQueryable<ExternalEvent> query, SyncRunId id)
    {
        return query.Where(i => i.SyncRunId == id);
    }
}
