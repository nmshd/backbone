using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(FileId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
