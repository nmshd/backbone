using Backbone.Messages.Domain.Ids;

namespace Backbone.Messages.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(MessageId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
