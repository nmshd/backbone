using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(RelationshipChangeId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}

