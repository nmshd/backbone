using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(MessageId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
