using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class SyncRunsQueryableExtensions
{
    extension(IQueryable<SyncRun> query)
    {
        public IQueryable<SyncRun> CreatedBy(IdentityAddress createdBy)
        {
            return query.Where(d => d.CreatedBy == createdBy);
        }

        public IQueryable<SyncRun> NotFinalized()
        {
            return query.Where(d => d.FinalizedAt == null);
        }

        public async Task<SyncRun> GetFirst(CancellationToken cancellationToken)
        {
            var syncRun = await query.FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(nameof(SyncRun));
            return syncRun;
        }
    }
}
