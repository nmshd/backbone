using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Backbone.Modules.Files.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

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
