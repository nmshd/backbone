﻿using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class PnsRegistrationsRepository : IPnsRegistrationsRepository
{
    private readonly DbSet<PnsRegistration> _registrations;
    private readonly IQueryable<PnsRegistration> _readonlyRegistrations;
    private readonly DevicesDbContext _dbContext;

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

    public async Task<IEnumerable<PnsRegistration>> FindWithAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _registrations : _readonlyRegistrations)
            .Where(registration => registration.IdentityAddress == address).ToListAsync(cancellationToken);
    }

    public async Task<PnsRegistration?> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _registrations : _readonlyRegistrations)
            .FirstOrDefaultAsync(registration => registration.DeviceId == deviceId, cancellationToken);
    }

    public async Task Delete(List<DeviceId> deviceIds, CancellationToken cancellationToken)
    {
        await _registrations.Where(x => deviceIds.Contains(x.DeviceId)).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task Update(PnsRegistration registration, CancellationToken cancellationToken)
    {
        _registrations.Update(registration);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
