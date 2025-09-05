using Backbone.Modules.Synchronization.Application.Infrastructure;
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
    }

    private async Task DeleteSyncRuns(CancellationToken cancellationToken)
    {
        var numberOfDeletedSyncRuns = await _dbContext.Set<SyncRun>().Where(SyncRun.CanBeCleanedUp).ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("Deleted {numberOfDeletedItems} sync runs", numberOfDeletedSyncRuns);
    }
}
