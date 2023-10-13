using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<RelationshipTemplateId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<RelationshipTemplateId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForRelationshipTemplateId(databaseId);
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

    public void ReportOrphanedDatabaseId(RelationshipTemplateId id)
    {
        _databaseIds.Add(id);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, RelationshipTemplateId, Exception> NO_BLOB_FOR_RELATIONSHIP_TEMPLATE_ID =
        LoggerMessage.Define<RelationshipTemplateId>(
            LogLevel.Error,
            new EventId(231727, "Relationships.SanityCheck.NoBlobForRelationshipTemplateId"),
            "No blob found for relationship template id: '{databaseId}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_DATABASE_ENTRY_FOR_BLOB_ID =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(232800, "Messages.SanityCheck.NoDatabaseEntryForBlobId"),
            "No database entry found for blob id: '{blobId}'."
        );

    public static void NoBlobForRelationshipTemplateId(this ILogger logger, RelationshipTemplateId relationshipTemplateId)
    {
        NO_BLOB_FOR_RELATIONSHIP_TEMPLATE_ID(logger, relationshipTemplateId, default!);
    }

    public static void NoDatabaseEntryForBlobId(this ILogger logger, string blobId)
    {
        NO_DATABASE_ENTRY_FOR_BLOB_ID(logger, blobId, default!);
    }
}
