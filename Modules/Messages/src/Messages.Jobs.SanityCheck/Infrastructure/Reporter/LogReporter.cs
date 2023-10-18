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
            MessagesLogs.NoBlobForMessageId(_logger, databaseId);
        }

        foreach (var blobId in _blobIds)
        {
            MessagesLogs.NoDatabaseEntryForBlobId(_logger, blobId);
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

internal static partial class MessagesLogs
{
    [LoggerMessage(
        EventId = 859729,
        EventName = "Messages.LogReporter.NoBlobForMessageId",
        Level = LogLevel.Error,
        Message = "No blob found for file id: '{databaseId}'.")]
    public static partial void NoBlobForMessageId(ILogger logger, MessageId databaseId);

    [LoggerMessage(
        EventId = 809167,
        EventName = "Messages.LogReporter.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(ILogger logger, string blobId);
}
