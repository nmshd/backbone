using Backbone.Modules.Messages.Domain.Ids;
using Messages.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Messages.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

public class TestReporter : IReporter
{
    public List<MessageId> ReportedDatabaseIds { get; } = new();
    public List<string> ReportedBlobIds { get; } = new();

    public void Complete()
    {
    }

    public void ReportOrphanedBlobId(string id)
    {
        ReportedBlobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(MessageId id)
    {
        ReportedDatabaseIds.Add(id);
    }
}
