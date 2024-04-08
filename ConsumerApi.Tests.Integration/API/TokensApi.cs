using Backbone.ConsumerApi.Tests.Integration.Models;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class TokensApi : BaseApi
{
    public TokensApi(HttpClientFactory factory) : base(factory) { }

    internal async Task<HttpResponse<CreateTokenResponse>> CreateToken(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateTokenResponse>("/Tokens", requestConfiguration);
    }

    internal async Task<HttpResponse<Token>> GetTokenById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Token>($"/Tokens/{id}", requestConfiguration);
    }

    internal async Task<HttpResponse<IEnumerable<Token>>> GetTokensByIds(RequestConfiguration requestConfiguration, IEnumerable<string> ids)
    {
        var endpoint = "/Tokens";
        var queryString = string.Join("&", ids.Select(id => $"ids={id}"));
        endpoint = $"{endpoint}?{queryString}";

        return await Get<IEnumerable<Token>>(endpoint, requestConfiguration);
    }
}
