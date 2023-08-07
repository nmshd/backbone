using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
public class DataSource : IDataSource
{
    private readonly QuotasDbContext _quotasDbContext;
    private readonly DevicesDbContext _devicesDbContext;

    public DataSource(QuotasDbContext quotasDbContext, DevicesDbContext devicesDbContext)
    {
        _quotasDbContext = quotasDbContext;
        _devicesDbContext = devicesDbContext;
    }

    public async Task<IEnumerable<string>> GetDevicesIdentitiesAddresses(CancellationToken cancellationToken)
    {
        return await _devicesDbContext.Identities.Select(i => i.Address).ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<string>> GetDevicesTiersIds(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>> GetQuotasIdentitiesAddresses(CancellationToken cancellationToken)
    {
        return await _quotasDbContext.Identities.Select(i => i.Address).ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<string>> GetQuotasTiersIds(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTierQuotaDefinitionIds(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> GetTierQuotasWithDefinitionIds(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
