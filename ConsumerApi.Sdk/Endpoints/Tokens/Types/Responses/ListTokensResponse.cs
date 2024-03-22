using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

public class ListTokensResponse(IEnumerable<Token> items) : EnumerableResponseBase<Token>(items);
