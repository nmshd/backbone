using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public interface IPnsRegistrationRepository
{
    Task Add(PnsRegistration registration, CancellationToken cancellationToken);
    Task Update(PnsRegistration registration, CancellationToken cancellationToken);
    Task<IEnumerable<PnsRegistration>> FindWithAddress(IdentityAddress address, CancellationToken cancellationToken);
    Task<PnsRegistration> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken);
}
