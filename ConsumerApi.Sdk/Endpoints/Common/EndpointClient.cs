using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

public class EndpointClient
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonSerializerOptions;
    protected readonly Authenticator _authenticator;

    public EndpointClient(HttpClient httpClient, Authenticator authenticator, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<ConsumerApiResponse<T>> PostUnauthenticated<T>(string url, object? requestContent = null)
    {
        return await Post<T>(url, requestContent, false);
    }

    public async Task<ConsumerApiResponse<T>> Post<T>(string url, object? requestContent = null)
    {
        return await Post<T>(url, requestContent, true);
    }

    private async Task<ConsumerApiResponse<T>> Post<T>(string url, object? requestContent, bool authenticate)
    {
        return await Execute<T>(HttpMethod.Post, url, requestContent, authenticate);
    }

    private async Task<ConsumerApiResponse<T>> Execute<T>(HttpMethod method, string url, object? requestContent, bool authenticate)
    {
        var httpRequest = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(requestContent),
        };

        if (authenticate)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _authenticator.GetJwt());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStreamAsync();
        var deserializedResponseContent = JsonSerializer.Deserialize<ConsumerApiResponse<T>>(responseContent, _jsonSerializerOptions)!;

        deserializedResponseContent.Status = response.StatusCode;

        return deserializedResponseContent;
    }
}
