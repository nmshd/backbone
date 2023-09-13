﻿using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ConsumerApi.Tests.Integration.Models;
using Enmeshed.BuildingBlocks.API;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ConsumerApi.Tests.Integration.API;

public class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    private readonly HttpClient _httpClient;
    private static AccessTokenResponse? _accessTokenResponse;

    protected BaseApi(HttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    protected async Task<HttpResponse<T>> Get<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(HttpMethod.Get, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse<T>> Post<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(HttpMethod.Post, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse<T>> ExecuteRequest<T>(HttpMethod method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new HttpRequestMessage(method, ROUTE_PREFIX + endpoint);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.Content = new StringContent(requestConfiguration.Content, MediaTypeHeaderValue.Parse(requestConfiguration.ContentType));

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.Headers.Add("Accept", requestConfiguration.AcceptHeader);

        if (requestConfiguration.Authenticate)
        {
            var tokenResponse = await GetAccessToken(requestConfiguration.AuthenticationParameters);
            request.Headers.Add("Authorization", $"Bearer {tokenResponse.AccessToken}");
        }

        var httpResponse = await _httpClient.SendAsync(request);
        var responseRawContent = await httpResponse.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<ResponseContent<T>>(responseRawContent);

        var response = new HttpResponse<T>
        {
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
            StatusCode = httpResponse.StatusCode,
            Content = responseData!,
            ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
            RawContent = responseRawContent
        };

        return response;
    }

    private async Task<AccessTokenResponse> GetAccessToken(AuthenticationParameters authenticationParams)
    {
        if (_accessTokenResponse is { IsExpired: false })
        {
            return _accessTokenResponse;
        }

        var form = new Dictionary<string, string>()
        {
            { "grant_type", authenticationParams.GrantType },
            { "username", authenticationParams.Username },
            { "password", authenticationParams.Password },
            { "client_id", authenticationParams.ClientId },
            { "client_secret",  authenticationParams.ClientSecret }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token") { Content = new FormUrlEncodedContent(form) };
        var httpResponse = await _httpClient.SendAsync(request);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorMessage = httpResponse.Content.ReadAsAsync<HttpError>()?.Result.Message ?? "Unknown error occurred when requesting an access token.";
            throw new AccessTokenRequestException(httpResponse.StatusCode, errorMessage);
        }

        var responseRawContent = await httpResponse.Content.ReadAsStringAsync();
        _accessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseRawContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        return _accessTokenResponse;
    }
}
