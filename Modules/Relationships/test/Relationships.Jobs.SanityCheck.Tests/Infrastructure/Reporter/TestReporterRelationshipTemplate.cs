using Backbone.Relationships.Domain.Ids;
using Backbone.Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.Reporter;

namespace Backbone.Relationships.Jobs.SanityCheck.Tests.Infrastructure.Reporter;

public class TestReporterRelationshipTemplate : IReporter
{
    public List<RelationshipTemplateId> ReportedDatabaseIds { get; } = new();
    public List<string> ReportedBlobIds { get; } = new();

    public void Complete()
    {
    }

    public void ReportOrphanedBlobId(string id)
    {
        ReportedBlobIds.Add(id);
    }

    public void ReportOrphanedDatabaseId(RelationshipTemplateId id)
    {
        ReportedDatabaseIds.Add(id);
    }
}
