using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Jobs.SanityCheck.Infrastructure.Reporter;

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

file static class LoggerExtensions
{
    private static readonly Action<ILogger, DatawalletModificationId, Exception> NO_BLOB_FOR_DATAWALLET_MODIFICATION_ID =
        LoggerMessage.Define<DatawalletModificationId>(
            LogLevel.Error,
            new EventId(525684, "Synchronization.LogReporter.NoBlobForDatawalletModificationId"),
            "No blob found for datawallet modification id: '{databaseId}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_DATABASE_ENTRY_FOR_BLOB_ID =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(560290, "Synchronization.LogReporter.NoDatabaseEntryForBlobId"),
            "No database entry found for blob id: '{blobId}'."
        );

    public static void NoBlobForDatawalletModificationId(this ILogger logger, DatawalletModificationId datawalletModificationId)
    {
        NO_BLOB_FOR_DATAWALLET_MODIFICATION_ID(logger, datawalletModificationId, default!);
    }

    public static void NoDatabaseEntryForBlobId(this ILogger logger, string blobId)
    {
        NO_DATABASE_ENTRY_FOR_BLOB_ID(logger, blobId, default!);
    }
}
