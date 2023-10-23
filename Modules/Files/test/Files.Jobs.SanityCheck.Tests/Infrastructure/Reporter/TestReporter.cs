using Backbone.Files.Domain.Entities;
using Backbone.Files.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Backbone.Files.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

public class TestReporter : IReporter
{
    public List<FileId> ReportedDatabaseIds { get; } = new();
    public List<string> ReportedBlobIds { get; } = new();

    public void Complete()
    {
    }

    public void ReportOrphanedBlobId(string id)
    {
        ReportedBlobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(FileId id)
    {
        ReportedDatabaseIds.Add(id);
    }
}
