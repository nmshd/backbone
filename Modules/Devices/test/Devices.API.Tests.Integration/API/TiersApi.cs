using System.Net;
using Devices.API.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace Devices.API.Tests.Integration.API;
public class TiersApi
{
    private const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;

    public TiersApi(RestClient client)
    {
        _client = client;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<HttpResponse<ListTiersResponse>> GetTiersList(RequestConfiguration requestConfiguration)
    {
        return await ExecuteTiersRequest<ListTiersResponse>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/Tiers").ToString(), requestConfiguration);
    }

    private async Task<HttpResponse<T>> ExecuteTiersRequest<T>(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(endpoint, method);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.AddBody(requestConfiguration.Content);

        if (!string.IsNullOrEmpty(requestConfiguration.ContentType))
            request.AddHeader("Content-Type", requestConfiguration.ContentType);

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.AddHeader("Accept", requestConfiguration.AcceptHeader);

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
}
