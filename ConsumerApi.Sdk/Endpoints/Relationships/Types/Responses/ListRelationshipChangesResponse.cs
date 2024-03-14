using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;

public class ListRelationshipChangesResponse(IEnumerable<RelationshipChange> items) : EnumerableResponseBase<RelationshipChange>(items);
