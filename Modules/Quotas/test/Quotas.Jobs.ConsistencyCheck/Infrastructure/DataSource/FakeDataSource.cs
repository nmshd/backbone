using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
