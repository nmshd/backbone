using System.Net;
using ConsumerApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace ConsumerApi.Tests.Integration.API;

public class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;
    private static AccessTokenResponse? _accessTokenResponse;

    protected BaseApi(RestClient client)
    {
        _client = client;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    protected async Task<HttpResponse<T>> Get<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Get, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse<T>> Post<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Post, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse<T>> ExecuteRequest<T>(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(new PathString(ROUTE_PREFIX).Add(endpoint).Value, method);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.AddBody(requestConfiguration.Content);

        if (!string.IsNullOrEmpty(requestConfiguration.ContentType))
            request.AddHeader("Content-Type", requestConfiguration.ContentType);

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.AddHeader("Accept", requestConfiguration.AcceptHeader);

        if (requestConfiguration.Authenticate)
        {
            var tokenResponse = await GetAccessToken(requestConfiguration.AuthenticationParameters);
            request.AddHeader("Authorization", $"Bearer {tokenResponse.AccessToken}");
        }

        var response = await _client.ExecuteAsync<ResponseContent<T>>(request);

        var result = new HttpResponse<T>
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Content = response.Data!,
            ContentType = response.ContentType,
            RawContent = response.Content
        };

        return result;
    }

    private async Task<AccessTokenResponse> GetAccessToken(AuthenticationParameters authenticationParams)
    {
        if (_accessTokenResponse is { IsExpired: false })
        {
            return _accessTokenResponse;
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

        _accessTokenResponse = response.Data!;

        return _accessTokenResponse;
    }
}