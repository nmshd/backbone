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

file static class LoggerExtensions
{
    private static readonly Action<ILogger, RelationshipChangeId, Exception> NO_BLOB_FOR_RELATIONSHIP_CHANGE_ID =
        LoggerMessage.Define<RelationshipChangeId>(
            LogLevel.Error,
            new EventId(349287, "Relationships.LogReporter.NoBlobForRelationshipChangeId"),
            "No blob found for relationship change id: '{databaseId}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_DATABASE_ENTRY_FOR_BLOB_ID =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(429922, "Relationships.LogReporter.NoDatabaseEntryForBlobId"),
            "No database entry found for blob id: '{blobId}'."
        );

    public static void NoBlobForRelationshipChangeId(this ILogger logger, RelationshipChangeId relationshipChangeId)
    {
        NO_BLOB_FOR_RELATIONSHIP_CHANGE_ID(logger, relationshipChangeId, default!);
    }

    public static void NoDatabaseEntryForBlobId(this ILogger logger, string blobId)
    {
        NO_DATABASE_ENTRY_FOR_BLOB_ID(logger, blobId, default!);
    }
}
