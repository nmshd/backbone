using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<RelationshipChangeId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<RelationshipChangeId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForRelationshipChangeId(databaseId);
        }

        foreach (var blobId in _blobIds)
        {
            _logger.NoDatabaseEntryForBlobId(blobId);
        }
    }

    public void ReportOrphanedBlobId(string id)
    {
        _blobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(RelationshipChangeId id)
    {
        _databaseIds.Add(id);
    }
}

internal static partial class RelationshipChangeLogs
{
    [LoggerMessage(
        EventId = 349287,
        EventName = "Relationships.LogReporter.NoBlobForRelationshipChangeId",
        Level = LogLevel.Error,
        Message = "No blob found for relationship change id: '{databaseId}'.")]
    public static partial void NoBlobForRelationshipChangeId(this ILogger logger, RelationshipChangeId databaseId);

    [LoggerMessage(
        EventId = 429922,
        EventName = "Relationships.LogReporter.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(this ILogger logger, string blobId);
}
