using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class SyncRunsQueryableExtensions
{
    public static IQueryable<SyncRun> WithId(this IQueryable<SyncRun> query, SyncRunId id)
    {
        return query.Where(d => d.Id == id);
    }

    public static IQueryable<SyncRun> CreatedBy(this IQueryable<SyncRun> query, IdentityAddress createdBy)
    {
        return query.Where(d => d.CreatedBy == createdBy);
    }

    public static IQueryable<SyncRun> NotFinalized(this IQueryable<SyncRun> query)
    {
        return query.Where(d => d.FinalizedAt == null);
    }

    public static async Task<SyncRun> GetFirst(this IQueryable<SyncRun> query, CancellationToken cancellationToken)
    {
        var syncRun = await query.FirstOrDefaultAsync(cancellationToken);

        if (syncRun == null)
            throw new NotFoundException(nameof(SyncRun));

        return syncRun;
    }
}
