using Backbone.Tokens.Domain.Entities;

namespace Backbone.Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly List<TokenId> _databaseIds;
    private readonly List<string> _blobIds;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _databaseIds = new List<TokenId>();
        _blobIds = new List<string>();
    }

    public void Complete()
    {
        foreach (var databaseId in _databaseIds)
        {
            _logger.NoBlobForTokenId(databaseId);
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

    public void ReportOrphanedDatabaseId(TokenId id)
    {
        _databaseIds.Add(id);
    }
}

internal static partial class LogReporterLogs
{
    [LoggerMessage(
        EventId = 826083,
        EventName = "Tokens.SanityCheck.NoBlobForTokenId",
        Level = LogLevel.Error,
        Message = "No blob found for token id: '{tokenId}'.")]
    public static partial void NoBlobForTokenId(this ILogger logger, TokenId tokenId);

    [LoggerMessage(
        EventId = 271286,
        EventName = "Tokens.SanityCheck.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(this ILogger logger, string blobId);
}
