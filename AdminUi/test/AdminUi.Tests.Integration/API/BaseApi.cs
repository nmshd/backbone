using System.Net;
using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using HttpResponse = AdminUi.Tests.Integration.Models.HttpResponse;

namespace AdminUi.Tests.Integration.API;

public class BaseApi
{
    private readonly RestClient _client;

    protected const string ROUTE_PREFIX = "/api/v1";
    private const string XSRF_TOKEN_HEADER_NAME = "X-XSRF-TOKEN";
    private const string XSRF_TOKEN_COOKIE_NAME = "X-XSRF-COOKIE";

    private string _xsrfToken = string.Empty;
    private string _xsrfCookie = string.Empty;

    protected BaseApi(IOptions<HttpClientOptions> httpConfiguration)
    {
        _client = new RestClient(httpConfiguration.Value.BaseUrl);
        _client.AddDefaultHeader("X-API-KEY", httpConfiguration.Value.ApiKey);

        LoadAndAddXSRFHeaders();

        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
    }

    private void LoadAndAddXSRFHeaders()
    {
        Task.Run(LoadXSRFTokensAsync).Wait();
        _client.AddDefaultHeader(XSRF_TOKEN_HEADER_NAME, _xsrfToken);
        _client.AddDefaultHeader("Cookie", $"{XSRF_TOKEN_COOKIE_NAME}={_xsrfCookie}");
    }

    private async Task LoadXSRFTokensAsync()
    {
        var token = await Get<string>("/xsrf", new() { AcceptHeader = "text/plain" });
        if (token.RawContent != null && token.Cookies != null && token.Cookies.Count > 0)
        {
            var cookie = token.Cookies.Single(c => c.Name == XSRF_TOKEN_COOKIE_NAME);
            _xsrfCookie = cookie.Value;
            _xsrfToken = token.RawContent;
        }
        else
        {
            throw new ValidationTestException("Could not load XSRF tokens.");
        }
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

    protected async Task<HttpResponse<T>> Patch<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(Method.Patch, endpoint, requestConfiguration);
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

        var restResponse = await _client.ExecuteAsync(request);
        var response = new HttpResponse
        {
            Content = JsonConvert.DeserializeObject<ErrorResponseContent>(restResponse.Content!)!,
            ContentType = restResponse.ContentType,
            IsSuccessStatusCode = restResponse.IsSuccessStatusCode,
            StatusCode = restResponse.StatusCode
        };

        return response;
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

        var restResponse = await _client.ExecuteAsync<ResponseContent<T>>(request);

        var response = new HttpResponse<T>
        {
            IsSuccessStatusCode = restResponse.IsSuccessStatusCode,
            StatusCode = restResponse.StatusCode,
            Content = restResponse.Data!,
            ContentType = restResponse.ContentType,
            RawContent = restResponse.Content,
            Cookies = restResponse.Cookies
        };

        return response;
    }
}
