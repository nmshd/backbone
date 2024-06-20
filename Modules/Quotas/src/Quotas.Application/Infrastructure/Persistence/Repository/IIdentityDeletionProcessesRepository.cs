using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IIdentityDeletionProcessesRepository
{
    Task<uint> CountInStatus(string createdBy, DateTime createdAtFrom, DateTime createdAtTo, DeletionProcessStatus status, CancellationToken cancellationToken);
}
