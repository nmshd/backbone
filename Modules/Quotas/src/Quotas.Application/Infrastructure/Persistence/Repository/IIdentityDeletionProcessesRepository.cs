namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IIdentityDeletionProcessesRepository
{
    Task<uint> CountInStatus(string createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
