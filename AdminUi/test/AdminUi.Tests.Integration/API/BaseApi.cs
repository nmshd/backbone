using System.Net;
using System.Net.Http.Headers;
using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HttpResponse = AdminUi.Tests.Integration.Models.HttpResponse;

namespace AdminUi.Tests.Integration.API;

public class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    private readonly HttpClient _httpClient;
    private const string XSRF_TOKEN_HEADER_NAME = "X-XSRF-TOKEN";
    private const string XSRF_TOKEN_COOKIE_NAME = "X-XSRF-COOKIE";
    private string _xsrfToken = string.Empty;
    private string _xsrfCookie = string.Empty;

    protected BaseApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", httpConfiguration.Value.ApiKey);

        LoadAndAddXSRFHeaders();

        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
    }

    private void LoadAndAddXSRFHeaders()
    {
        Task.Run(LoadXSRFTokensAsync).Wait();
        _httpClient.DefaultRequestHeaders.Add(XSRF_TOKEN_HEADER_NAME, _xsrfToken);
        _httpClient.DefaultRequestHeaders.Add("Cookie", $"{XSRF_TOKEN_COOKIE_NAME}={_xsrfCookie}");
    }

    private async Task LoadXSRFTokensAsync()
    {
        var token = await Get<string>("/xsrf", new() { AcceptHeader = "text/plain" });
        if (token is { RawContent: not null, Cookies.Count: > 0 })
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
        return await ExecuteRequest<T>(HttpMethod.Get, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse<T>> Post<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(HttpMethod.Post, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse> Post(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest(HttpMethod.Post, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse> Delete(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest(HttpMethod.Delete, endpoint, requestConfiguration);
    }

    protected async Task<HttpResponse<T>> Patch<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(HttpMethod.Patch, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse> ExecuteRequest(HttpMethod method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new HttpRequestMessage(method, ROUTE_PREFIX + endpoint);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.Content = new StringContent(requestConfiguration.Content, MediaTypeHeaderValue.Parse(requestConfiguration.ContentType));

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.Headers.Add("Accept", requestConfiguration.AcceptHeader);

        var httpResponse = await _httpClient.SendAsync(request);

        var response = new HttpResponse
        {
            Content = JsonConvert.DeserializeObject<ErrorResponseContent>(await httpResponse.Content.ReadAsStringAsync())!,
            ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
            StatusCode = httpResponse.StatusCode
        };

        return response;
    }

    private async Task<HttpResponse<T>> ExecuteRequest<T>(HttpMethod method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new HttpRequestMessage(method, ROUTE_PREFIX + endpoint);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
            request.Content = new StringContent(requestConfiguration.Content, MediaTypeHeaderValue.Parse(requestConfiguration.ContentType));

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.Headers.Add("Accept", requestConfiguration.AcceptHeader);

        var httpResponse = await _httpClient.SendAsync(request);
        var responseRawContent = await httpResponse.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<ResponseContent<T>>(responseRawContent);

        var response = new HttpResponse<T>
        {
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
            StatusCode = httpResponse.StatusCode,
            Content = responseData!,
            ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
            RawContent = responseRawContent,
            Cookies = httpResponse.Headers.GetValues("Set-Cookie")?.Select(it => new Models.Cookie() { Name = it, Value = it }).ToList().AsReadOnly()
        };

        return response;
    }
}
