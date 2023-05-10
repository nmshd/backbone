using System.Net;
using AdminApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace AdminApi.Tests.Integration.API;

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
        var request = new RestRequest(new PathString(ROUTE_PREFIX).Add(endpoint).ToString(), Method.Get);

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