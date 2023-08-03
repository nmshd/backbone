using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<string> DevicesIdentitiesIds { get; } = new ();

    public List<string> QuotasIdentitiesIds { get; } = new();

    public Task<IEnumerable<string>> GetDevicesIdentitiesIds()
    {
        return Task.FromResult(DevicesIdentitiesIds.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetDevicesTiersIds()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetQuotasIdentitiesIds()
    {
        return Task.FromResult(QuotasIdentitiesIds.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetQuotasTiersIds()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTierQuotaDefinitionIds()
    {
        throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> GetTierQuotasWithDefinitionIds()
    {
        throw new NotImplementedException();
    }
}
