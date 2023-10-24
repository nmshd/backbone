using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(TokenId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
