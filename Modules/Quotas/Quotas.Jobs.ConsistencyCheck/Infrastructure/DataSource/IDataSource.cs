namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public interface IDataSource
{
    Task<IEnumerable<string>> GetDevicesIdentitiesIds();
    Task<IEnumerable<string>> GetQuotasIdentitiesIds();

    Task<IEnumerable<string>> GetDevicesTiersIds();
    Task<IEnumerable<string>> GetQuotasTiersIds();

    Task<IEnumerable<string>> GetTierQuotaDefinitionIds();
    Task<IDictionary<string, string>> GetTierQuotasWithDefinitionIds();

}
