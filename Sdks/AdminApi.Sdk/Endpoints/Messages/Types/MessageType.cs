namespace Backbone.AdminApi.Sdk.Endpoints.Messages.Types;

public record MessageType
{
    public static readonly MessageType INCOMING = new("Incoming");
    public static readonly MessageType OUTGOING = new("Outgoing");

    public MessageType(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
