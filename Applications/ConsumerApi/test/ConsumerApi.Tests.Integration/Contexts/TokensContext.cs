using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class TokensContext
{
    public Dictionary<string, CreateTokenResponse> CreateTokenResponses { get; } = new();
}
