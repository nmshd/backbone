using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class DevicesRepository : IDevicesRepository
{
    private readonly DbSet<Device> _devices;
    private readonly IQueryable<Device> _readonlyDevices;
    private readonly DevicesDbContext _dbContext;

    public DevicesRepository(DevicesDbContext dbContext)
    {
        _devices = dbContext.Devices;
        _readonlyDevices = dbContext.Devices.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task<DbPaginationResult<Device>> FindAll(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter)
    {
        var query = _readonlyDevices
            .NotDeleted()
            .IncludeAll(_dbContext)
            .OfIdentity(identity);

        if (ids.Any())
            query = query.WithIdIn(ids);

        return await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

    }

    public async Task<Device> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _devices : _readonlyDevices)
            .NotDeleted()
            .IncludeAll(_dbContext)
            .FirstWithId(deviceId, cancellationToken);
    }

    public async Task MarkAsDeleted(DeviceId id, byte[] deletionCertificate, DeviceId deletedByDevice, CancellationToken cancellationToken)
    {
        var device = await _devices.FirstWithId(id, cancellationToken);

        device.MarkAsDeleted(deletionCertificate, deletedByDevice);
        _devices.Update(device);

        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Device device, CancellationToken cancellationToken)
    {
        _devices.Update(device);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
