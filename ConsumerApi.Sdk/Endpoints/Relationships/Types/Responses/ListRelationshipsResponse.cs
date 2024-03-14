using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;

public class ListRelationshipsResponse(IEnumerable<Relationship> items) : EnumerableResponseBase<Relationship>(items);
