using System.Net;
using Devices.API.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace Devices.API.Tests.Integration.API;
public class IdentitiesApi
{
    private const string ROUTE_PREFIX = "/api/v1";
    private readonly RestClient _client;

    public IdentitiesApi(RestClient client)
    {
        _client = client;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<HttpResponse<ListIdentitiesResponse>> GetIdentitiesList(RequestConfiguration requestConfiguration)
    {
        return await ExecuteIdentitiesRequest<ListIdentitiesResponse>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/Identities").ToString(), requestConfiguration);
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
