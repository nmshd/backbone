using Backbone.Modules.Messages.Domain.Ids;

namespace Messages.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(MessageId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}