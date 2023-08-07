using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
using Identity = Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity;
using Tier = Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    #region Identities

    public List<string> DevicesIdentitiesIds { get; } = new();
    public List<string> QuotasIdentitiesIds { get; } = new();

    public Task<IEnumerable<string>> GetDevicesIdentitiesAddresses(CancellationToken cancellationToken)
    {
        return Task.FromResult(DevicesIdentitiesIds.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetQuotasIdentitiesAddresses(CancellationToken cancellationToken)
    {
        return Task.FromResult(QuotasIdentitiesIds.AsEnumerable());
    }

    #endregion

    #region Tiers
    public List<string> DevicesTiersIds { get; } = new();
    public List<string> QuotasTiersIds { get; } = new();


    public Task<IEnumerable<string>> GetDevicesTiersIds(CancellationToken cancellationToken)
    {
        return Task.FromResult(DevicesTiersIds.AsEnumerable());
    }


    public Task<IEnumerable<string>> GetQuotasTiersIds(CancellationToken cancellationToken)
    {
        return Task.FromResult(QuotasTiersIds.AsEnumerable());
    }

    #endregion

    #region TierQuotas vs TierQuotaDefinitions
    public List<Tier> Tiers { get; set; } = new();
    public List<Identity> Identities { get; set; } = new();

    public Task<IEnumerable<Tier>> GetTiers(CancellationToken cancellationToken)
    {
        return Task.FromResult(Tiers.AsEnumerable());
    }

    public Task<IEnumerable<Identity>> GetIdentities(CancellationToken cancellationToken)
    {
        return Task.FromResult(Identities.AsEnumerable());
    }
    #endregion
}
