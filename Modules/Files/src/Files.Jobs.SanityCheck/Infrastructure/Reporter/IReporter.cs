using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(FileId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
