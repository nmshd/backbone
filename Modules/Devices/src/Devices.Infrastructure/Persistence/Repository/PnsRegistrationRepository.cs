using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class PnsRegistrationRepository : IPnsRegistrationRepository
{
    private readonly DbSet<PnsRegistration> _registrations;
    private readonly IQueryable<PnsRegistration> _readonlyRegistrations;
    private readonly DevicesDbContext _dbContext;

    public PnsRegistrationRepository(DevicesDbContext dbContext)
    {
        _registrations = dbContext.PnsRegistrations;
        _readonlyRegistrations = dbContext.PnsRegistrations.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task Add(PnsRegistration registration, CancellationToken cancellationToken)
    {
        await _registrations.AddAsync(registration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<PnsRegistration>> FindWithAddress(IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readonlyRegistrations.Where(registration => registration.IdentityAddress == address).ToListAsync(cancellationToken);
    }

    public async Task<PnsRegistration> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken)
    {
        return await _readonlyRegistrations.FirstOrDefaultAsync(registration =>  registration.DeviceId == deviceId, cancellationToken);
    }

    public async Task Update(PnsRegistration registration, CancellationToken cancellationToken)
    {
        _registrations.Update(registration);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
