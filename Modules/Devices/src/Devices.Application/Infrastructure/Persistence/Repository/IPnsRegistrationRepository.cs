using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Devices.Application.Infrastructure.Persistence.Repository;

public interface IPnsRegistrationRepository
{
    Task Add(PnsRegistration registration, CancellationToken cancellationToken);
    Task Update(PnsRegistration registration, CancellationToken cancellationToken);
    Task<IEnumerable<PnsRegistration>> FindWithAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false);
    Task<PnsRegistration> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task Delete(List<DeviceId> deviceIds, CancellationToken cancellationToken);
}
