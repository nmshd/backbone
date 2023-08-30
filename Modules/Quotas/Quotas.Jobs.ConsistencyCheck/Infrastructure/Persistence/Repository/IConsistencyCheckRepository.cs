namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;
public interface IConsistencyCheckRepository
{
    Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken);
}
