namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class CreateRelationshipRequest
{
    public required string RelationshipTemplateId { get; set; }
    public byte[]? Content { get; set; }
}
