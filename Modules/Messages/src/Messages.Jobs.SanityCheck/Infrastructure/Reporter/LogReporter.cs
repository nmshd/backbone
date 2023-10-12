using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Jobs.SanityCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<MessageId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<MessageId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForMessageId(databaseId);
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

    public void ReportOrphanedDatabaseId(MessageId id)
    {
        _databaseIds.Add(id);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, MessageId, Exception> NO_BLOB_FOR_MESSAGE_ID =
        LoggerMessage.Define<MessageId>(
            LogLevel.Error,
            new EventId(859729, "Messages.SanityCheck.NoBlobForMessageId"),
            "No blob found for file id: '{databaseId}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_DATABASE_ENTRY_FOR_BLOB_ID =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(809167, "Messages.SanityCheck.NoDatabaseEntryForBlobId"),
            "No database entry found for blob id: '{blobId}'."
        );

    public static void NoBlobForMessageId(this ILogger logger, MessageId messageId)
    {
        NO_BLOB_FOR_MESSAGE_ID(logger, messageId, default!);
    }

    public static void NoDatabaseEntryForBlobId(this ILogger logger, string blobId)
    {
        NO_DATABASE_ENTRY_FOR_BLOB_ID(logger, blobId, default!);
    }
}
