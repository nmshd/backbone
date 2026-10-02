namespace Backbone.Modules.Quotas.Abstractions;

public interface IIdentityDeletionProcessesRepository
{
    Task<uint> CountInStatus(string createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
