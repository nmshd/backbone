namespace Backbone.ConsumerApi.Sdk.Endpoints.Notifications.Types;

public class SendNotificationRequest
{
    public required string Code { get; init; }
    public List<string> Recipients { get; init; } = [];
}
