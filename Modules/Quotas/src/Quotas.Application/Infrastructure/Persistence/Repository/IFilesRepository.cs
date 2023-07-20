using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface IFilesRepository
{
    Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
