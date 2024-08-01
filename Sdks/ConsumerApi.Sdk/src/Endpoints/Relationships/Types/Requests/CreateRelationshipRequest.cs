namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;

public class CreateRelationshipRequest
{
    public required string RelationshipTemplateId { get; set; }
    public byte[]? Content { get; set; }
}
