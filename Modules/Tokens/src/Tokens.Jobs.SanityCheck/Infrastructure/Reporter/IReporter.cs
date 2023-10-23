using Backbone.Tokens.Domain.Entities;

namespace Backbone.Tokens.Jobs.SanityCheck.Infrastructure.Reporter;

public interface IReporter
{
    void ReportOrphanedDatabaseId(TokenId id);

    void ReportOrphanedBlobId(string id);

    void Complete();
}
