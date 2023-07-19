using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface IFilesRepository
{
    Task<uint> Count(string uploader, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
    Task<uint> UsedSpace(string uploader, DateTime from, DateTime to, CancellationToken cancellationToken);
}
