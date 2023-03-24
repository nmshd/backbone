﻿using System.Net;
using Devices.API.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace Challenges.API.Tests.Integration.API;
public class IdentitiesApi
{
    private const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;
    private static AccessTokenResponse? _accessTokenResponse;

    public IdentitiesApi(RestClient client)
    {
        _client = client;
        _accessTokenResponse = null;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<HttpResponse<List<IdentityDTO>>> GetIdentitiesList(RequestConfiguration requestConfiguration)
    {
        return await ExecuteIdentitiesRequest<List<IdentityDTO>>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/identities").ToString(), requestConfiguration);
    }

    private async Task<HttpResponse<T>> ExecuteIdentitiesRequest<T>(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(endpoint, method);

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

        var response = await _client.ExecuteAsync<T>(request);

        var result = new HttpResponse<T>
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Data = response.Data,
            ContentType = response.ContentType,
            Content = response.Content
        };

        return result;
    }

    public async Task<AccessTokenResponse> GetAccessToken(AuthenticationParameters authenticationParams)
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
