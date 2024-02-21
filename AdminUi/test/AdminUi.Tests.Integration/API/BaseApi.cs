using System.Net;
using System.Net.Http.Headers;
using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HttpResponse = Backbone.AdminUi.Tests.Integration.Models.HttpResponse;

namespace Backbone.AdminUi.Tests.Integration.API;

internal class BaseApi
{
    protected const string ROUTE_PREFIX = "/api/v1";
    protected const string ODATA_ROUTE_PREFIX = "/odata";
    private readonly HttpClient _httpClient;
    private const string XSRF_TOKEN_HEADER_NAME = "X-XSRF-TOKEN";
    private const string XSRF_TOKEN_COOKIE_NAME = "X-XSRF-COOKIE";
    private string _xsrfToken = string.Empty;
    private string _xsrfCookie = string.Empty;

    protected BaseApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", httpConfiguration.Value.ApiKey);

        LoadAndAddXsrfHeaders();

        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
    }

    private void LoadAndAddXsrfHeaders()
    {
        Task.Run(LoadXsrfTokensAsync).Wait();
        _httpClient.DefaultRequestHeaders.Add(XSRF_TOKEN_HEADER_NAME, _xsrfToken);
        _httpClient.DefaultRequestHeaders.Add("Cookie", $"{XSRF_TOKEN_COOKIE_NAME}={_xsrfCookie}");
    }

    private async Task LoadXsrfTokensAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ROUTE_PREFIX + "/xsrf");
        request.Headers.Add("Accept", "text/plain");

        var httpResponse = await _httpClient.SendAsync(request);

        if (httpResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            _xsrfToken = await httpResponse.Content.ReadAsStringAsync();
            _xsrfCookie = cookies.Select(it =>
            {
                var rawCookieHeader = it.Split('=', 2);
                return new Models.Cookie { Name = rawCookieHeader[0], Value = rawCookieHeader[1] };
            }).First(c => c.Name == XSRF_TOKEN_COOKIE_NAME).Value;
        }
        else
        {
            throw new ValidationTestException("Could not load XSRF tokens.");
        }
    }

    protected async Task<ODataResponse<T>> GetOData<T>(string endpoint)
    {
        return await ExecuteODataRequest<T>(HttpMethod.Get, endpoint);
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

    protected async Task<HttpResponse<T>> Put<T>(string endpoint, RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<T>(HttpMethod.Put, endpoint, requestConfiguration);
    }

    private async Task<HttpResponse> ExecuteRequest(HttpMethod method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new HttpRequestMessage(method, ROUTE_PREFIX + endpoint);

        if (string.IsNullOrEmpty(requestConfiguration.ContentType))
            throw new ArgumentNullException(nameof(requestConfiguration.ContentType));

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

    private async Task<ODataResponse<T>> ExecuteODataRequest<T>(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, ODATA_ROUTE_PREFIX + endpoint);

        var httpResponse = await _httpClient.SendAsync(request);
        var responseRawContent = await httpResponse.Content.ReadAsStringAsync();

        var responseData = JsonConvert.DeserializeObject<ODataResponseContent<T>>(responseRawContent);

        var response = new ODataResponse<T>
        {
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
            StatusCode = httpResponse.StatusCode,
            Content = responseData!,
            ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
            RawContent = responseRawContent
        };

        return response;
    }

    private async Task<HttpResponse<T>> ExecuteRequest<T>(HttpMethod method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new HttpRequestMessage(method, ROUTE_PREFIX + endpoint);

        if (string.IsNullOrEmpty(requestConfiguration.ContentType))
            throw new ArgumentNullException(nameof(requestConfiguration.ContentType));

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
            RawContent = responseRawContent
        };

        return response;
    }
}
