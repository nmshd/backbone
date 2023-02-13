using Backbone.Modules.Synchronization.Domain.Entities;

namespace Synchronization.Jobs.SanityCheck.Infrastructure.Reporter
{
    public interface IReporter
    {
        void ReportOrphanedDatabaseId(DatawalletModificationId id);

        void ReportOrphanedBlobId(string id);

        void Complete();
    }
}
