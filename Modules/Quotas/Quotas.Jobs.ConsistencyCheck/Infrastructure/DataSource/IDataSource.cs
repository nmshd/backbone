namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public interface IDataSource
{
    Task<IEnumerable<string>> GetDevicesIdentitiesAddresses(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetQuotasIdentitiesAddresses(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetDevicesTiersIds(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetQuotasTiersIds(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetTierQuotaDefinitionIds(CancellationToken cancellationToken);
    Task<IDictionary<string, string>> GetTierQuotasWithDefinitionIds(CancellationToken cancellationToken);

}
