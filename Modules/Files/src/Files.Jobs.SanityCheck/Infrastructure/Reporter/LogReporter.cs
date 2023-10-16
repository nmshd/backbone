using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<FileId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<FileId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForFileId(databaseId);
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

    public void ReportOrphanedDatabaseId(FileId id)
    {
        _databaseIds.Add(id);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, FileId, Exception> NO_BLOB_FOR_FILE_ID =
        LoggerMessage.Define<FileId>(
            LogLevel.Error,
            new EventId(629592, "Files.LogReporter.NoBlobForFileId"),
            "No blob found for file id: '{databaseId}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_DATABASE_ENTRY_FOR_BLOB_ID =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(487180, "Files.LogReporter.NoDatabaseEntryForBlobId"),
            "No database entry found for blob id: '{blobId}'."
        );

    public static void NoBlobForFileId(this ILogger logger, FileId fileId)
    {
        NO_BLOB_FOR_FILE_ID(logger, fileId, default!);
    }

    public static void NoDatabaseEntryForBlobId(this ILogger logger, string blobId)
    {
        NO_DATABASE_ENTRY_FOR_BLOB_ID(logger, blobId, default!);
    }
}
