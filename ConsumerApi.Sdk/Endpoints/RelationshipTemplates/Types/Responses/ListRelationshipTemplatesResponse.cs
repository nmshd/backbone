using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

public class ListRelationshipTemplatesResponse(IEnumerable<RelationshipTemplate> items) : EnumerableResponseBase<RelationshipTemplate>(items);
