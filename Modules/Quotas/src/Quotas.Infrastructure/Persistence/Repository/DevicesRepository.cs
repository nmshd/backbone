using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class DevicesRepository : IDevicesRepository
{
    private readonly IQueryable<Device> _readOnlyDevices;

    public DevicesRepository(QuotasDbContext dbContext)
    {
        _readOnlyDevices = dbContext.Devices.AsNoTracking();
    }

    public async Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var count = await _readOnlyDevices
            .CreatedInInterval(from, to)
            .CountAsync(d => d.IdentityAddress == identityAddress, cancellationToken);

        return (uint)count;
    }
}
