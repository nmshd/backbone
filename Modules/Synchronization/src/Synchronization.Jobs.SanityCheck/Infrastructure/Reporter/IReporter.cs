using Backbone.Synchronization.Domain.Entities;

namespace Backbone.Synchronization.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(DatawalletModificationId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
