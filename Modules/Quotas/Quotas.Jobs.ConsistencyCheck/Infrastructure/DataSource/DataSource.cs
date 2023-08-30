using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public class DataSource : IDataSource
{
    private readonly IConsistencyCheckRepository _consistencyCheckRepository;

    public DataSource(IConsistencyCheckRepository consistencyCheckRepository)
    {
        _consistencyCheckRepository = consistencyCheckRepository;
    }

    public Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken)
    {
        return _consistencyCheckRepository.GetIdentitiesMissingFromQuotas(cancellationToken);
    }

    public Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken)
    {
        return _consistencyCheckRepository.GetIdentitiesMissingFromDevices(cancellationToken);
    }

    public Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken)
    {
        return _consistencyCheckRepository.GetTiersMissingFromQuotas(cancellationToken);
    }

    public Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken)
    {
        return _consistencyCheckRepository.GetTiersMissingFromDevices(cancellationToken);
    }
}
