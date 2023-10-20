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

internal static partial class LogReporterLogs
{
    [LoggerMessage(
        EventId = 231727,
        EventName = "Relationships.SanityCheck.NoBlobForRelationshipTemplateId",
        Level = LogLevel.Error,
        Message = "No blob found for relationship template id: '{databaseId}'.")]
    public static partial void NoBlobForRelationshipTemplateId(this ILogger logger, RelationshipTemplateId databaseId);

    [LoggerMessage(
        EventId = 232800,
        EventName = "Relationships.SanityCheck.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(this ILogger logger, string blobId);
}
