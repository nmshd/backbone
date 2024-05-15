namespace Backbone.AdminApi.Sdk.Endpoints.Messages.Types;

public record MessageType
{
    public static readonly MessageType INCOMING = new("Incoming");
    public static readonly MessageType OUTGOING = new("Outgoing");

    public string Value { get; }

    public MessageType(string value)
    {
        Value = value;
    }
}
