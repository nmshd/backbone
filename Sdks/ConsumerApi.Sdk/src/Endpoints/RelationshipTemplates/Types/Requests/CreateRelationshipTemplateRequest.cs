namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;

public class CreateRelationshipTemplateRequest
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public required byte[] Content { get; set; }
    public string? ForIdentity { get; set; }
    public byte[]? Password { get; set; }
}
