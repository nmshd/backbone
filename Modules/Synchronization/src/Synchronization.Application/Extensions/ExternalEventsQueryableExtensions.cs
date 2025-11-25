using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class ExternalEventsQueryableExtensions
{
    extension(IQueryable<ExternalEvent> query)
    {
        public IQueryable<ExternalEvent> WithOwner(IdentityAddress owner)
        {
            return query.Where(d => d.Owner == owner);
        }

        public IQueryable<ExternalEvent> WithErrorCountBelow(byte maxErrorCount)
        {
            return query.Where(d => d.SyncErrorCount < maxErrorCount);
        }

        public IQueryable<ExternalEvent> Unsynced()
        {
            return query.Where(e => e.SyncRunId == null);
        }

        public IQueryable<ExternalEvent> NotBlocked()
        {
            return query.Where(e => !e.IsDeliveryBlocked);
        }

        public IQueryable<ExternalEvent> Blocked()
        {
            return query.Where(e => e.IsDeliveryBlocked);
        }

        public IQueryable<ExternalEvent> WithType(ExternalEventType type)
        {
            return query.Where(e => e.Type == type);
        }

        public IQueryable<ExternalEvent> WithContext(string context)
        {
            return query.Where(e => e.Context == context);
        }

        public IQueryable<ExternalEvent> AssignedToSyncRun(SyncRunId id)
        {
            return query.Where(i => i.SyncRunId == id);
        }
    }
}
