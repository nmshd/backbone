using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

public class EndpointClient
{
    private const string EMPTY_RESULT = """
    {
        "result": {}
    }
    """;

    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonSerializerOptions;
    protected readonly Authenticator _authenticator;

    public EndpointClient(HttpClient httpClient, Authenticator authenticator, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<ConsumerApiResponse<T>> Post<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Post, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();
    public async Task<ConsumerApiResponse<T>> PostUnauthenticated<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Post, url)
        .WithJson(requestContent)
        .Execute();

    public async Task<ConsumerApiResponse<T>> Get<T>(string url, object? requestContent = null, PaginationFilter? pagination = null) => await Request<T>(HttpMethod.Get, url)
        .Authenticate()
        .WithPagination(pagination)
        .WithJson(requestContent)
        .Execute();
    public async Task<ConsumerApiResponse<T>> GetUnauthenticated<T>(string url, object? requestContent = null, PaginationFilter? pagination = null) => await Request<T>(HttpMethod.Get, url)
        .WithPagination(pagination)
        .WithJson(requestContent)
        .Execute();

    public async Task<ConsumerApiResponse<T>> Put<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Put, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();
    public async Task<ConsumerApiResponse<T>> PutUnauthenticated<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Put, url)
        .WithJson(requestContent)
        .Execute();

    public async Task<ConsumerApiResponse<T>> Delete<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Delete, url)
        .Authenticate()
        .WithJson(requestContent)
        .Execute();
    public async Task<ConsumerApiResponse<T>> DeleteUnauthenticated<T>(string url, object? requestContent = null) => await Request<T>(HttpMethod.Delete, url)
        .WithJson(requestContent)
        .Execute();

    public RequestBuilder<T> Request<T>(HttpMethod method, string url) => RequestBuilder<T>.Create(this, _jsonSerializerOptions, method, url);

    private async Task<ConsumerApiResponse<T>> Execute<T>(HttpMethod method, string url, HttpContent content, bool authenticate, NameValueCollection extraHeaders)
    {
        var (responseContent, statusCode) = await ExecuteIntern(method, url, content, authenticate, extraHeaders);
        if (responseContent.Length == 0) //If no content is sent, automatically decode an "empty" result
        {
            responseContent.Close();
            responseContent = new MemoryStream(Encoding.UTF8.GetBytes(EMPTY_RESULT));
        }
        var deserializedResponseContent = JsonSerializer.Deserialize<ConsumerApiResponse<T>>(responseContent, _jsonSerializerOptions)!;

        deserializedResponseContent.Status = statusCode;

        return deserializedResponseContent;
    }

    private async Task<RawConsumerApiResponse> ExecuteRaw(HttpMethod method, string url, HttpContent content, bool authenticate, NameValueCollection extraHeaders)
    {
        var (responseContent, statusCode) = await ExecuteIntern(method, url, content, authenticate, extraHeaders);

        MemoryStream mem = new();
        await responseContent.CopyToAsync(mem);
        var data = mem.ToArray();

        ConsumerApiError? error = null;
        if (statusCode >= HttpStatusCode.BadRequest) //In case of an error, deserialize it
        {
            mem.Seek(0, SeekOrigin.Begin);
            var deserialized = JsonSerializer.Deserialize<EmptyConsumerApiResponse>(mem, _jsonSerializerOptions)!;
            error = deserialized.Error;
        }

        return new RawConsumerApiResponse { Content = data, Error = error, Status = statusCode };
    }

    private async Task<(Stream, HttpStatusCode)> ExecuteIntern(HttpMethod method, string url, HttpContent content, bool authenticate, NameValueCollection extraHeaders)
    {
        var httpRequest = new HttpRequestMessage(method, url)
        {
            Content = content
        };

        if (authenticate)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _authenticator.GetJwt());

        foreach (var k in extraHeaders.AllKeys)
        {
            var values = extraHeaders.GetValues(k);
            if (k != null && values != null) httpRequest.Headers.Add(k, values);
        }

        var response = await _httpClient.SendAsync(httpRequest);
        return (await response.Content.ReadAsStreamAsync(), response.StatusCode);
    }

    public class RequestBuilder<T>
    {
        private readonly EndpointClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private readonly string _url;
        private readonly HttpMethod _method;
        private bool _authenticated;
        private HttpContent _content;
        private readonly NameValueCollection _extraHeaders = [];
        private readonly NameValueCollection _queryParameters = [];

        public static RequestBuilder<T> Create(EndpointClient client, JsonSerializerOptions jsonSerializerOptions, HttpMethod method, string url)
            => new(client, jsonSerializerOptions, method, url);

        private RequestBuilder(EndpointClient client, JsonSerializerOptions jsonSerializerOptions, HttpMethod method, string url)
        {
            _client = client;
            _jsonSerializerOptions = jsonSerializerOptions;

            _url = url;
            _method = method;
            _authenticated = false;
            _content = JsonContent.Create((object?)null);
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

        public async Task<ConsumerApiResponse<T>> Execute() => await _client.Execute<T>(_method, EncodeParametersInUrl(), _content, _authenticated, _extraHeaders);

        public async Task<RawConsumerApiResponse> ExecuteRaw() => await _client.ExecuteRaw(_method, EncodeParametersInUrl(), _content, _authenticated, _extraHeaders);

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
