using System.Net;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Tokens.API.Tests.Integration.Models;

namespace Tokens.API.Tests.Integration.API;
public class TokensApi
{
    private const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;
    private static readonly Dictionary<string, AccessTokenResponse> UserAccessTokens = new();

    public TokensApi(RestClient client)
    {
        _client = client;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<HttpResponse<Response<Token>>> CreateToken(RequestConfiguration requestConfiguration)
    {
        return await ExecuteTokenRequest<Response<Token>>(Method.Post, new PathString(ROUTE_PREFIX).Add("/tokens").ToString(), requestConfiguration);
    }

    public async Task<HttpResponse<Response<Token>>> GetTokenById(RequestConfiguration requestConfiguration, string id)
    {
        return await ExecuteTokenRequest<Response<Token>>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/tokens/{id}").ToString(), requestConfiguration);
    }

    public async Task<HttpResponse<Response<IEnumerable<Token>>>> GetTokenById(RequestConfiguration requestConfiguration, IEnumerable<string> ids)
    {
        var endpoint = new PathString(ROUTE_PREFIX).Add("/tokens").ToString();
        var queryString = string.Join("&", ids.Select(id => $"ids={id}"));
        endpoint = $"{endpoint}?{queryString}";

        return await ExecuteTokenRequest<Response<IEnumerable<Token>>>(Method.Get, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse<T>> ExecuteTokenRequest<T>(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(endpoint, method);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.AddJsonBody(requestConfiguration.Content);

        if (!string.IsNullOrEmpty(requestConfiguration.ContentType))
            request.AddHeader("Content-Type", requestConfiguration.ContentType);

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.AddHeader("Accept", requestConfiguration.AcceptHeader);

        if (requestConfiguration.Authenticate)
        {
            var tokenResponse = await GetAccessToken(requestConfiguration.AuthenticationParameters);
            request.AddHeader("Authorization", $"Bearer {tokenResponse.AccessToken}");
        }

        var response = await _client.ExecuteAsync<T>(request);

        var result = new HttpResponse<T>
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            HttpMethod = response.Request!.Method.ToString(),
            StatusCode = response.StatusCode,
            Data = response.Data,
            ContentType = response.ContentType,
            Content = response.Content
        };

        return result;
    }

    private async Task<AccessTokenResponse> GetAccessToken(AuthenticationParameters authenticationParams)
    {
        if (UserAccessTokens.TryGetValue(authenticationParams.Username, out var accessToken))
        {
            if (accessToken is { IsExpired: false })
            {
                return accessToken;
            }
        }

        var request = new RestRequest("/connect/token", Method.Post);

        request.AddParameter("grant_type", authenticationParams.GrantType);
        request.AddParameter("username", authenticationParams.Username);
        request.AddParameter("password", authenticationParams.Password);
        request.AddParameter("client_id", authenticationParams.ClientId);
        request.AddParameter("client_secret", authenticationParams.ClientSecret);

        var response = await _client.ExecuteAsync<AccessTokenResponse>(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = response.ErrorMessage?.ToString() ?? "Unknown error occurred when requesting an access token.";
            throw new AccessTokenRequestException(response.StatusCode, errorMessage);
        }

        var newAccessToken = response.Data!;

        UserAccessTokens[authenticationParams.Username] = newAccessToken;

        return newAccessToken;
    }
}
