using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Backbone.Tooling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly DevicesDbContext _dbContext;
    private readonly DbSet<Device> _devices;
    private readonly IQueryable<Device> _readonlyDevices;
    private readonly IServiceProvider _serviceProvider;

    public IdentitiesRepository(DevicesDbContext dbContext, IServiceProvider serviceProvider)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _dbContext = dbContext;
        _devices = dbContext.Devices;
        _readonlyDevices = dbContext.Devices.AsNoTracking();
        _serviceProvider = serviceProvider;
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var paginationResult = await _readonlyIdentities
            .IncludeAll(_dbContext)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);
        return paginationResult;
    }

    public async Task<Identity> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identities : _readonlyIdentities)
            .IncludeAll(_dbContext)
            .FirstWithAddressOrDefault(address, cancellationToken);
    }

    public async Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readonlyIdentities
            .AnyAsync(i => i.Address == address, cancellationToken);
    }

    public async Task AddUser(ApplicationUser user, string password)
    {
        var createUserResult = await _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>().CreateAsync(user, password);
        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));
    }
    public async Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var query = _readonlyDevices
            .NotDeleted()
            .IncludeAll(_dbContext)
            .OfIdentity(identity);

        if (ids.Any())
            query = query.WithIdIn(ids);

        return await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);
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

    public async Task Update(Identity identity, CancellationToken cancellationToken)
    {
        _identities.Update(identity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Identity>> FindAllActiveWithPastDeletionGracePeriod(CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identities : _readonlyIdentities)
            .IncludeAll(_dbContext)
            .Where(i => i.Status == IdentityStatus.Active && i.DeletionGracePeriodEndsAt != null && i.DeletionGracePeriodEndsAt < SystemTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task Delete(Identity identity, CancellationToken cancellationToken)
    {
        _identities.Remove(identity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
