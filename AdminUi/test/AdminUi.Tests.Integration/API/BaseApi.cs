using System.Net;
using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using HttpResponse = AdminUi.Tests.Integration.Models.HttpResponse;

namespace AdminUi.Tests.Integration.API;

public class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;

    protected BaseApi(IOptions<HttpClientOptions> httpConfiguration)
    {
        _client = new RestClient(httpConfiguration.Value.BaseUrl);
        _client.AddDefaultHeader("X-API-KEY", httpConfiguration.Value.ApiKey);

        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
    }

    protected async Task<HttpResponse<T>> Get<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Get, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse<T>> Post<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Post, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse> Delete(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest(Method.Delete, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse> ExecuteRequest(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(new PathString(ROUTE_PREFIX).Add(endpoint).Value, method);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.AddBody(requestConfiguration.Content);

        if (!string.IsNullOrEmpty(requestConfiguration.ContentType))
            request.AddHeader("Content-Type", requestConfiguration.ContentType);

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.AddHeader("Accept", requestConfiguration.AcceptHeader);

        var response = await _client.ExecuteAsync(request);
        var result = new HttpResponse();
        if (!response.IsSuccessful)
            result = JsonConvert.DeserializeObject<HttpResponse>(response.Content!);

        result!.IsSuccessStatusCode = response.IsSuccessStatusCode;
        result.StatusCode = response.StatusCode;
        result.RawContent = response.Content;

        return result;
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
}
