using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class RelationshipsContext
{
    public readonly Dictionary<string, Relationship> Relationships = new();
    public readonly Dictionary<string, CreateRelationshipTemplateResponse> CreateRelationshipTemplateResponses = new();
}
