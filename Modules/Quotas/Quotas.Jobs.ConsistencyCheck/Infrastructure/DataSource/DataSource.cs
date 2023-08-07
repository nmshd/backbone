using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
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
        return await _devicesDbContext.Identities.AsNoTracking().Select(i => i.Address).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetDevicesTiersIds(CancellationToken cancellationToken)
    {
        return await _devicesDbContext.Tiers.AsNoTracking().Select(i => i.Id).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetQuotasIdentitiesAddresses(CancellationToken cancellationToken)
    {
        return await _quotasDbContext.Identities.AsNoTracking().Select(i => i.Address).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetQuotasTiersIds(CancellationToken cancellationToken)
    {
        return await _quotasDbContext.Tiers.AsNoTracking().Select(i => i.Id.Value).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tier>> GetTiers(CancellationToken cancellationToken)
    {
        return await _quotasDbContext.Tiers.IncludeAll(_quotasDbContext).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Identity>> GetIdentities(CancellationToken cancellationToken)
    {
        return await _quotasDbContext.Identities.AsNoTracking().IncludeAll(_quotasDbContext).ToListAsync(cancellationToken);
    }
}
