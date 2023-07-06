using System.Net;
using AdminUi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace AdminUi.Tests.Integration.API;

public class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;

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

    protected async Task<HttpResponse<T>> Delete<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Delete, endpoint, requestConfiguration);
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