namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;

public class DatawalletModification
{
    public required string Id { get; set; }
    public required ushort DatawalletVersion { get; set; }
    public required long Index { get; set; }
    public required string ObjectIdentifier { get; set; }
    public string? PayloadCategory { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public required string Collection { get; set; }
    public required string Type { get; set; }
    public byte[]? EncryptedPayload { get; set; }
}
