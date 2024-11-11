using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class RelationshipsContext
{
    public readonly Dictionary<string, Relationship> Relationships = new();
}
