using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Devices.Infrastructure.PushNotifications.DirectPush.Responses;

namespace Backbone.Devices.Infrastructure.PushNotifications.DirectPush;

public interface IPnsConnector
{
    Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification);
    void ValidateRegistration(PnsRegistration registration);
}
