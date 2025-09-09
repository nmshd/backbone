using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteSyncRuns(cancellationToken);
        await DeleteDatawalletModifications(cancellationToken);
    }

    private async Task DeleteSyncRuns(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _dbContext.Set<SyncRun>().Where(SyncRun.CanBeCleanedUp).ExecuteDeleteAsync(cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "sync runs", stopwatch.ElapsedMilliseconds);
    }

    private async Task DeleteDatawalletModifications(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _dbContext.Set<DatawalletModification>().Where(DatawalletModification.CanBeCleanedUp).ExecuteDeleteAsync(cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "datawallet modifications", stopwatch.ElapsedMilliseconds);
    }
}
