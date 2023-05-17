using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Identity;
using Backbone.Modules.Devices.Application;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly DevicesDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DbSet<Device> _devices;
    private readonly IQueryable<Device> _readonlyDevices;

    public IdentitiesRepository(DevicesDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _dbContext = dbContext;
        _userManager = userManager;
        _devices = dbContext.Devices;
        _readonlyDevices = dbContext.Devices.AsNoTracking();
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter)
    {
        var paginationResult = await _readonlyIdentities
            .IncludeAll(_dbContext)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter);
        return paginationResult;
    }

    public async Task<Identity> FindByAddress(IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readonlyIdentities
            .IncludeAll(_dbContext)
            .FirstWithAddressOrDefault(address, cancellationToken);
    }

    public async Task AddUser(ApplicationUser user, string password)
    {
        var createUserResult = await _userManager.CreateAsync(user, password);
        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));
    }
    public async Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter)
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

    public async Task Update(Device device, CancellationToken cancellationToken)
    {
        _devices.Update(device);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
