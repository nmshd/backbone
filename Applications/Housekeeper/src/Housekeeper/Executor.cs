using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;

namespace Backbone.Housekeeper;

public class Executor
{
    private readonly IEnumerable<IHousekeeper> _housekeepers;
    private readonly ILogger<Executor> _logger;

    public Executor(IEnumerable<IHousekeeper> housekeepers, ILogger<Executor> logger)
    {
        _housekeepers = housekeepers;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _logger.StartingDeletion();
        foreach (var housekeeper in _housekeepers)
        {
            var result = await housekeeper.Execute(cancellationToken);

            foreach (var resultItem in result.Items)
            {
                _logger.LogResult(resultItem.NumberOfDeletedEntities, resultItem.EntityType.Name);
            }
        }
    }
}

internal static partial class ExecutorLogs
{
    [LoggerMessage(
        EventId = 468524,
        EventName = "Housekeeper.Executor.StartingDeletion",
        Level = LogLevel.Information,
        Message = "Starting deletion...")]
    public static partial void StartingDeletion(this ILogger logger);

    [LoggerMessage(
        EventId = 864565,
        EventName = "Housekeeper.Executor.ResultItem",
        Level = LogLevel.Information,
        Message = "Deleted {count} items of type '{itemType}'.")]
    public static partial void LogResult(this ILogger logger, int count, string itemType);
}
