using Backbone.Modules.Tokens.Domain.Entities;
using Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Tokens.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

public class TestReporter : IReporter
{
    public List<TokenId> ReportedDatabaseIds { get; } = new();
    public List<string> ReportedBlobIds { get; } = new();

    public void Complete()
    {
    }

    public void ReportOrphanedBlobId(string id)
    {
        ReportedBlobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(TokenId id)
    {
        ReportedDatabaseIds.Add(id);
    }
}