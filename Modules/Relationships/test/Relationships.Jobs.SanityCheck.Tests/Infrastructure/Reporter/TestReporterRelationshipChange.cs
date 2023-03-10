using Backbone.Modules.Relationships.Domain.Ids;
using Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.Reporter;

namespace Relationships.Jobs.SanityCheck.Tests.Infrastructure.Reporter
{
    public class TestReporterRelationshipChange : IReporter
    {
        public List<RelationshipChangeId> ReportedDatabaseIds { get; } = new();
        public List<string> ReportedBlobIds { get; } = new();

        public void Complete()
        {
        }

        public void ReportOrphanedBlobId(string id)
        {
            ReportedBlobIds.Add(id);
        }

        public void ReportOrphanedDatabaseId(RelationshipChangeId id)
        {
            ReportedDatabaseIds.Add(id);
        }
    }
}
