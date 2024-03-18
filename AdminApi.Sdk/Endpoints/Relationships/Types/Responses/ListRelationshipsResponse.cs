using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Relationships.Types.Responses;

public class ListRelationshipsResponse(IEnumerable<Relationship> items) : EnumerableResponseBase<Relationship>(items);
