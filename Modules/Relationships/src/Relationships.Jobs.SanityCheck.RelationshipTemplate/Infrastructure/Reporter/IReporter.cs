using Backbone.Modules.Relationships.Domain.Ids;

namespace Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(RelationshipTemplateId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
