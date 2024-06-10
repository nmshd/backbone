using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public interface IPnsConnector
{
    Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, IPushNotification notification);
    void ValidateRegistration(PnsRegistration registration);
}
