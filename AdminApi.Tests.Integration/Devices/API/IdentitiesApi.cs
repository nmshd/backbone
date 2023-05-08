using System.Net;
using AdminApi.Tests.Integration.Models;
using AdminApi.Tests.Integration.Utils.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace AdminApi.Tests.Integration.API;

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

    public async Task<HttpResponse<List<IdentitySummaryDTO>>> GetIdentities(RequestConfiguration requestConfiguration)
    {
        return await ExecuteIdentitiesRequest<List<IdentitySummaryDTO>>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/Identities").ToString(), requestConfiguration);
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
