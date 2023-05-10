using ConsumerApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace ConsumerApi.Tests.Integration.API;

public class TokensApi : BaseApi
{
    public TokensApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<Token>> CreateToken(RequestConfiguration requestConfiguration)
    {
        return await Post<Token>("/tokens", requestConfiguration);
    }

    public async Task<HttpResponse<Token>> GetTokenById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Token>($"/tokens/{id}", requestConfiguration);
    }

    public async Task<HttpResponse<IEnumerable<Token>>> GetTokensByIds(RequestConfiguration requestConfiguration, IEnumerable<string> ids)
    {
        var endpoint = new PathString(ROUTE_PREFIX).Add("/tokens").ToString();
        var queryString = string.Join("&", ids.Select(id => $"ids={id}"));
        endpoint = $"{endpoint}?{queryString}";

        return await Get<IEnumerable<Token>>(endpoint, requestConfiguration);
    }
}
