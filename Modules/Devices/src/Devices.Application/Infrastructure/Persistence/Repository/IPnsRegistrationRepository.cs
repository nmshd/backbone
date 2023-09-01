using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IPnsRegistrationRepository
{
    Task Add(PnsRegistration registration, CancellationToken cancellationToken);
    Task Update(PnsRegistration registration, CancellationToken cancellationToken);
    Task<IEnumerable<PnsRegistration>> FindWithAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false);
    Task<PnsRegistration> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
}
