using Backbone.Synchronization.Domain.Entities;
using Backbone.Synchronization.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Backbone.Synchronization.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

public class TestReporter : IReporter
{
    public List<DatawalletModificationId> ReportedDatabaseIds { get; } = new();
    public List<string> ReportedBlobIds { get; } = new();

    public void Complete()
    {
    }

    public void ReportOrphanedBlobId(string id)
    {
        ReportedBlobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(DatawalletModificationId id)
    {
        ReportedDatabaseIds.Add(id);
    }
}
