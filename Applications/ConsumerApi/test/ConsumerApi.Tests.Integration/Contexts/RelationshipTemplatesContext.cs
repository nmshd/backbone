using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class RelationshipTemplatesContext
{
    public Dictionary<string, CreateRelationshipTemplateResponse> CreateRelationshipTemplatesResponses { get; } = new();
}
