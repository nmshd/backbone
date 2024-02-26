namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public class NotificationTextAttribute : Attribute
{
    public const string DEFAULT_TITLE = "Aktualisierungen eingegangen";
    public const string DEFAULT_BODY = "Es sind neue Aktualisierungen in der Enmeshed App vorhanden.";

    public required string Title { get; set; }
    public required string Body { get; set; }
}
