namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
public class CreateRelationshipTemplateRequest
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public required byte[] Content { get; set; }
}
