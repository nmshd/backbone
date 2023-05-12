﻿using ConsumerApi.Tests.Integration.Models;
using RestSharp;

namespace ConsumerApi.Tests.Integration.API;

public class TokensApi : BaseApi
{
    public TokensApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<CreateTokenResponse>> CreateToken(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateTokenResponse>("/Tokens", requestConfiguration);
    }

    public async Task<HttpResponse<Token>> GetTokenById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Token>($"/Tokens/{id}", requestConfiguration);
    }

    public async Task<HttpResponse<IEnumerable<Token>>> GetTokensByIds(RequestConfiguration requestConfiguration, IEnumerable<string> ids)
    {
        var endpoint = "/Tokens";
        var queryString = string.Join("&", ids.Select(id => $"ids={id}"));
        endpoint = $"{endpoint}?{queryString}";

        return await Get<IEnumerable<Token>>(endpoint, requestConfiguration);
    }
}
