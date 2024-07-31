using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;

public interface IPnsConnector
{
    Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IPushNotification notification);
    void ValidateRegistration(PnsRegistration registration);
}
