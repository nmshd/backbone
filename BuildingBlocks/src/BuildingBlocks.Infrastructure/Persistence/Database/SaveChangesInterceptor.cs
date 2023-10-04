using Enmeshed.Tooling.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;

public class SaveChangesTimeInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<SaveChangesTimeInterceptor> _logger;
    private System.Diagnostics.Stopwatch _stopwatch;

    public SaveChangesTimeInterceptor(ILogger<SaveChangesTimeInterceptor> logger)
    {
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        _stopwatch.Stop();
        _logger.LogInformation(LogEventIds.EXECUTION_TIME, "'SaveChangesAsync' took {elapsedMilliseconds} ms.", _stopwatch.ElapsedMilliseconds);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
