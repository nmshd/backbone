namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public interface IPnsConnector
{
    Task Send(IEnumerable<PnsRegistration> registrations, object notification);
}
