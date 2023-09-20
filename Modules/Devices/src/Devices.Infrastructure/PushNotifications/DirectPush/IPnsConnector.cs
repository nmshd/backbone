using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public interface IPnsConnector
{
    Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification);
    void ValidateRegistration(PnsRegistration registration);
}
