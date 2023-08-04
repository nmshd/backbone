using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    #region Identities

    public List<string> DevicesIdentitiesIds { get; } = new();
    public List<string> QuotasIdentitiesIds { get; } = new();

    public Task<IEnumerable<string>> GetDevicesIdentitiesIds()
    {
        return Task.FromResult(DevicesIdentitiesIds.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetQuotasIdentitiesIds()
    {
        return Task.FromResult(QuotasIdentitiesIds.AsEnumerable());
    }

    #endregion

    #region Tiers
    public List<string> DevicesTiersIds { get; } = new();
    public List<string> QuotasTiersIds { get; } = new();


    public Task<IEnumerable<string>> GetDevicesTiersIds()
    {
        return Task.FromResult(DevicesTiersIds.AsEnumerable());
    }


    public Task<IEnumerable<string>> GetQuotasTiersIds()
    {
        return Task.FromResult(QuotasTiersIds.AsEnumerable());
    }

    #endregion

    public Task<IEnumerable<string>> GetTierQuotaDefinitionIds()
    {
        throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> GetTierQuotasWithDefinitionIds()
    {
        throw new NotImplementedException();
    }
}
