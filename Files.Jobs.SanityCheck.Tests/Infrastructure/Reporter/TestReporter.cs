using Backbone.Modules.Files.Domain.Entities;
using Files.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Files.Jobs.SanityCheck.Tests.Infrastructure.Reporter
{
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
}
