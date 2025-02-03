using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class PnsRegistrationsRepository : IPnsRegistrationsRepository
{
    private readonly DevicesDbContext _dbContext;
    private readonly IQueryable<PnsRegistration> _readonlyRegistrations;
    private readonly DbSet<PnsRegistration> _registrations;

    public PnsRegistrationsRepository(DevicesDbContext dbContext)
    {
        _registrations = dbContext.PnsRegistrations;
        _readonlyRegistrations = dbContext.PnsRegistrations.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task Add(PnsRegistration registration, CancellationToken cancellationToken)
    {
        try
        {
            await _registrations.AddAsync(registration, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            if (exception.HasReason(DbUpdateExceptionReason.UniqueKeyViolation))
                throw new InfrastructureException(InfrastructureErrors.UniqueKeyViolation(registration.DeviceId));
        }
    }

    public async Task<PnsRegistration?> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _registrations : _readonlyRegistrations)
            .FirstOrDefaultAsync(registration => registration.DeviceId == deviceId, cancellationToken);
    }

    public async Task<PnsRegistration[]> FindByDeviceIds(DeviceId[] deviceIds, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _registrations : _readonlyRegistrations)
            .Where(r => deviceIds.Contains(r.DeviceId))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<int> Delete(List<DeviceId> deviceIds, CancellationToken cancellationToken)
    {
        var numberOfDeletedRows = await _registrations.Where(x => deviceIds.Contains(x.DeviceId)).ExecuteDeleteAsync(cancellationToken);
        return numberOfDeletedRows;
    }

    public async Task Update(PnsRegistration registration, CancellationToken cancellationToken)
    {
        _registrations.Update(registration);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Expression<Func<PnsRegistration, bool>> filter, CancellationToken cancellationToken)
    {
        await _registrations.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
