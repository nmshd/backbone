using Backbone.FilesSanityCheck.Infrastructure.Reporter;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Tests.Infrastructure.Reporter;

public class TestReporter : IReporter
{
    public List<FileId> ReportedDatabaseIds { get; } = [];
    public List<string> ReportedBlobIds { get; } = [];

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
