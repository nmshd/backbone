using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(RelationshipChangeId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}

