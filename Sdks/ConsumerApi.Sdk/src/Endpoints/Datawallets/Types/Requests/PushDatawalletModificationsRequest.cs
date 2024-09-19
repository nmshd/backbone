namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;

public class PushDatawalletModificationsRequest
{
    public required int LocalIndex { get; set; }
    public required List<PushDatawalletModificationsRequestItem> Modifications { get; set; }
}

public class PushDatawalletModificationsRequestItem
{
    /// <summary>
    /// Is usually one of the following: "Create", "Update", "Delete", "CacheChanged"
    /// </summary>
    public required string Type { get; set; }

    public required string ObjectIdentifier { get; set; }
    public required string Collection { get; set; }

    /// <summary>
    /// Is usually one of the following: "TechnicalData", "Userdata", "Metadata"
    /// </summary>
    public string? PayloadCategory { get; set; }

    public byte[]? EncryptedPayload { get; set; }
    public required ushort DatawalletVersion { get; set; }
}
