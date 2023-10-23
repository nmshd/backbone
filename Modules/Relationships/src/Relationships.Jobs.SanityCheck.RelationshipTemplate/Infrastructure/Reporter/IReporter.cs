using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(RelationshipTemplateId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
