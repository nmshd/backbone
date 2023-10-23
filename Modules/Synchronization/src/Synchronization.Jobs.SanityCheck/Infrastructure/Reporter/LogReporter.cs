using Backbone.Synchronization.Domain.Entities;

namespace Backbone.Synchronization.Jobs.SanityCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<DatawalletModificationId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<DatawalletModificationId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForDatawalletModificationId(databaseId);
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

    public void ReportOrphanedDatabaseId(DatawalletModificationId id)
    {
        _databaseIds.Add(id);
    }
}

internal static partial class LogReporterLogs
{
    [LoggerMessage(
        EventId = 525684,
        EventName = "Synchronization.SanityCheck.NoBlobForDatawalletModificationId",
        Level = LogLevel.Error,
        Message = "No blob found for datawallet modification id: '{databaseId}'.")]
    public static partial void NoBlobForDatawalletModificationId(this ILogger logger, DatawalletModificationId databaseId);

    [LoggerMessage(
        EventId = 560290,
        EventName = "Synchronization.SanityCheck.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(this ILogger logger, string blobId);
}
