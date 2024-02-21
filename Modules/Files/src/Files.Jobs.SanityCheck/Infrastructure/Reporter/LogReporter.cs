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

        _databaseIds = [];
        _blobIds = [];
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

internal static partial class LogReporterLogs
{
    [LoggerMessage(
        EventId = 629592,
        EventName = "Files.SanityCheck.NoBlobForFileId",
        Level = LogLevel.Error,
        Message = "No blob found for file id: '{databaseId}'.")]
    public static partial void NoBlobForFileId(this ILogger logger, FileId databaseId);

    [LoggerMessage(
        EventId = 487180,
        EventName = "Files.SanityCheck.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(this ILogger logger, string blobId);
}
