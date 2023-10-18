using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

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
            TokensLogs.NoBlobForTokenId(_logger, databaseId);
        }

        foreach (var blobId in _blobIds)
        {
            TokensLogs.NoDatabaseEntryForBlobId(_logger, blobId);
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

internal static partial class TokensLogs
{
    [LoggerMessage(
        EventId = 826083,
        EventName = "Tokens.LogReporter.NoBlobForTokenId",
        Level = LogLevel.Error,
        Message = "No blob found for token id: '{tokenId}'.")]
    public static partial void NoBlobForTokenId(ILogger logger, TokenId tokenId);

    [LoggerMessage(
        EventId = 271286,
        EventName = "Tokens.LogReporter.NoDatabaseEntryForBlobId",
        Level = LogLevel.Error,
        Message = "No database entry found for blob id: '{blobId}'.")]
    public static partial void NoDatabaseEntryForBlobId(ILogger logger, string blobId);
}
