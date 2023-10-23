using Backbone.Tokens.Domain.Entities;
using Backbone.Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Backbone.Tokens.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

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
