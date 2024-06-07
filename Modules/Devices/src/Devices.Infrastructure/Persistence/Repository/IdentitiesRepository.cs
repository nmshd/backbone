using System.Linq.Expressions;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly DevicesDbContext _dbContext;
    private readonly DbSet<Device> _devices;
    private readonly IQueryable<Device> _readonlyDevices;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IQueryable<IdentityDeletionProcessAuditLogEntry> _readonlyIdentityDeletionProcessAuditLogs;

    public IdentitiesRepository(DevicesDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _dbContext = dbContext;
        _devices = dbContext.Devices;
        _readonlyDevices = dbContext.Devices.AsNoTracking();
        _readonlyIdentityDeletionProcessAuditLogs = dbContext.IdentityDeletionProcessAuditLogs.AsNoTracking();
        _userManager = userManager;
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var paginationResult = await _readonlyIdentities
            .IncludeAll(_dbContext)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);
        return paginationResult;
    }

    public async Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identities : _readonlyIdentities)
            .IncludeAll(_dbContext)
            .FirstWithAddressOrDefault(address, cancellationToken);
    }

    public async Task<IEnumerable<IdentityDeletionProcessAuditLogEntry>> GetIdentityDeletionProcessAuditLogsByAddress(byte[] identityAddressHash, CancellationToken cancellationToken)
    {
        return await _readonlyIdentityDeletionProcessAuditLogs
            .Where(auditLog => auditLog.IdentityAddressHash == identityAddressHash)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readonlyIdentities
            .AnyAsync(i => i.Address == address, cancellationToken);
    }

    public async Task<IEnumerable<Identity>> FindAllWithDeletionProcessInStatus(DeletionProcessStatus status, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identities : _readonlyIdentities)
            .IncludeAll(_dbContext)
            .Where(i => i.DeletionProcesses.Any(d => d.Status == status))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByClientId(string clientId, CancellationToken cancellationToken)
    {
        return await _readonlyIdentities
            .CountAsync(i => i.ClientId == clientId, cancellationToken);
    }

    public async Task AddUser(ApplicationUser user, string password)
    {
        var createUserResult = await _userManager.CreateAsync(user, password);
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

    public async Task<Device?> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _devices : _readonlyDevices)
            .NotDeleted()
            .IncludeAll(_dbContext)
            .FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);
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

    public async Task<IEnumerable<Identity>> Find(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identities : _readonlyIdentities)
            .IncludeAll(_dbContext)
            .Where(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task Delete(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken)
    {
        await _identities.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
