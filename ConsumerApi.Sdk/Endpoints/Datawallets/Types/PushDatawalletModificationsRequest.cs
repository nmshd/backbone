namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;

public class PushDatawalletModificationsRequest
{
    public required int LocalIndex { get; set; }
    public required List<PushDatawalletModificationItem> Modifications { get; set; }
}

public class PushDatawalletModificationItem
{
    public required string Type { get; set; }
    public required string ObjectIdentifier { get; set; }
    public required string Collection { get; set; }
    public string? PayloadCategory { get; set; }
    public byte[]? EncryptedPayload { get; set; }
    public required ushort DatawalletVersion { get; set; }
}
