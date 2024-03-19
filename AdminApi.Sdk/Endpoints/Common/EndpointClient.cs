using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Common;

public class EndpointClient
{
    private const string EMPTY_RESULT = """
        {
            "result": {}
        }
        """;

    private readonly HttpClient _httpClient;
    private readonly XsrfAndApiKeyAuthenticator _authenticator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EndpointClient(HttpClient httpClient, XsrfAndApiKeyAuthenticator authenticator, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<AdminApiResponse<T>> Post<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Post, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();
    public async Task<AdminApiResponse<T>> PostUnauthenticated<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Post, url)
        .WithJson(requestContent)
        .Execute();

    public async Task<AdminApiResponse<T>> Get<T>(string url, object? requestContent = null, PaginationFilter? pagination = null) => await Request<T>(HttpMethod.Get, url)
        .Authenticate()
        .WithPagination(pagination)
        .WithJson(requestContent)
        .Execute();

    public async Task<AdminApiResponse<T>> Put<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Put, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();

    public async Task<AdminApiResponse<T>> Patch<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Patch, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();

    public async Task<AdminApiResponse<T>> Delete<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Delete, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();

    public RequestBuilder<T> Request<T>(HttpMethod method, string url) => new(this, _authenticator, _jsonSerializerOptions, method, url);

    private async Task<AdminApiResponse<T>> Execute<T>(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStreamAsync();
        var statusCode = response.StatusCode;

        if (responseContent.Length == 0) //If no content is sent, automatically decode an "empty" result
        {
            responseContent.Close();
            responseContent = new MemoryStream(Encoding.UTF8.GetBytes(EMPTY_RESULT));
        }

        var adminApiResponse = JsonSerializer.Deserialize<AdminApiResponse<T>>(responseContent, _jsonSerializerOptions)!;
        adminApiResponse.Status = statusCode;

        return adminApiResponse;
    }

    public class RequestBuilder<T>
    {
        private readonly EndpointClient _client;
        private readonly XsrfAndApiKeyAuthenticator _authenticator;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private readonly string _url;
        private readonly HttpMethod _method;
        private bool _authenticated;
        private HttpContent? _content;
        private readonly NameValueCollection _extraHeaders = [];
        private readonly NameValueCollection _queryParameters = [];

        public RequestBuilder(EndpointClient client, XsrfAndApiKeyAuthenticator authenticator, JsonSerializerOptions jsonSerializerOptions, HttpMethod method, string url)
        {
            _client = client;
            _authenticator = authenticator;
            _jsonSerializerOptions = jsonSerializerOptions;

            _url = url;
            _method = method;
            _authenticated = false;
        }

        public RequestBuilder<T> Authenticate(bool authenticate = true)
        {
            _authenticated = authenticate;
            return this;
        }

        public RequestBuilder<T> WithJson(object? content)
        {
            _content = JsonContent.Create(content, null, _jsonSerializerOptions);
            return this;
        }

        public RequestBuilder<T> WithUrlEncodedForm(Dictionary<string, string> formContent)
        {
            _content = new FormUrlEncodedContent(formContent);
            return this;
        }

        public RequestBuilder<T> WithMultipartForm(MultipartContent content)
        {
            _content = content;
            return this;
        }

        public RequestBuilder<T> WithPagination(PaginationFilter? pagination)
        {
            if (pagination != null) AddQueryParameters(pagination.ToQueryParameters());
            return this;
        }

        public RequestBuilder<T> AddQueryParameter(string key, string value)
        {
            _queryParameters.Add(key, value);
            return this;
        }

        public RequestBuilder<T> AddQueryParameter<TValue>(string key, TValue? value) where TValue : unmanaged
        {
            _queryParameters.Add(key, value.ToString());
            return this;
        }

        public RequestBuilder<T> AddQueryParameter(string key, IEnumerable<string> value)
        {
            foreach (var e in value) _queryParameters.Add(key, e);
            return this;
        }

        public RequestBuilder<T> AddQueryParameter(string key, object value)
        {
            _queryParameters.Add(key, JsonSerializer.SerializeToElement(value, _jsonSerializerOptions).ToString());
            return this;
        }

        public RequestBuilder<T> AddQueryParameters(NameValueCollection parameters)
        {
            _queryParameters.Add(parameters);
            return this;
        }

        public RequestBuilder<T> AddQueryParameters(IQueryParameterStorage storage) => AddQueryParameters(storage.ToQueryParameters());

        public RequestBuilder<T> AddExtraHeader(string key, string value)
        {
            _extraHeaders.Add(key, value);
            return this;
        }

        public RequestBuilder<T> AddExtraHeader(string key, List<string> value)
        {
            foreach (var e in value) _extraHeaders.Add(key, e);
            return this;
        }

        public RequestBuilder<T> AddExtraHeaders(NameValueCollection headers)
        {
            _extraHeaders.Add(headers);
            return this;
        }

        public async Task<AdminApiResponse<T>> Execute() => await _client.Execute<T>(await CreateRequestMessage());

        private async Task<HttpRequestMessage> CreateRequestMessage()
        {
            var request = new HttpRequestMessage(_method, EncodeParametersInUrl())
            {
                Content = _content
            };

            if (_authenticated) await _authenticator.Authenticate(request);

            foreach (var k in _extraHeaders.AllKeys)
            {
                var values = _extraHeaders.GetValues(k);
                if (k != null && values != null) request.Headers.Add(k, values);
            }

            return request;
        }

        private string EncodeParametersInUrl()
        {
            if (_queryParameters.Count == 0) return _url;

            var encodedEntries = _queryParameters.AllKeys
                .SelectMany(key => _queryParameters.GetValues(key) ?? [], (key, value) => (key, value))
                .Select(kv => $"{HttpUtility.UrlEncode(kv.key)}={HttpUtility.UrlEncode(kv.value)}")
                .ToList();

            return $"{_url}?{string.Join('&', encodedEntries)}";
        }
    }
}
