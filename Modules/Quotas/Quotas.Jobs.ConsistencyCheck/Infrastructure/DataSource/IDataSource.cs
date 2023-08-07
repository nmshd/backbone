using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public interface IDataSource
{
    Task<IEnumerable<string>> GetDevicesIdentitiesAddresses(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetQuotasIdentitiesAddresses(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetDevicesTiersIds(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetQuotasTiersIds(CancellationToken cancellationToken);

    Task<IEnumerable<Tier>> GetTiers(CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> GetIdentities(CancellationToken cancellationToken);

}
