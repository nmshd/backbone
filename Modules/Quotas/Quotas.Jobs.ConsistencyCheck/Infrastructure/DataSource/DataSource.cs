using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public class DataSource : IDataSource
{
    public Task<IEnumerable<string>> GetDevicesIdentitiesIds()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetDevicesTiersIds()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetQuotasIdentitiesIds()
    {
        throw new NotImplementedException();
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
