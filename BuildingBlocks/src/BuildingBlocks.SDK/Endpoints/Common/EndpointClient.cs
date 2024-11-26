using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common;

public class EndpointClient
{
    private const string EMPTY_RESULT = """
                                        {
                                            "result": {}
                                        }
                                        """;

    private const string EMPTY_VALUE = """
                                       {
                                           "value": {}
                                       }
                                       """;

    private readonly IAuthenticator _authenticator;

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EndpointClient(HttpClient httpClient, IAuthenticator authenticator, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<ApiResponse<T>> Post<T>(string url, object? requestContent = null)
    {
        return await Request<T>(HttpMethod.Post, url)
            .Authenticate()
            .WithJson(requestContent)
            .Execute();
    }

    public async Task<ApiResponse<T>> PostUnauthenticated<T>(string url, object? requestContent = null)
    {
        return await Request<T>(HttpMethod.Post, url)
            .WithJson(requestContent)
            .Execute();
    }

    public async Task<ApiResponse<T>> Get<T>(string url, object? requestContent = null, PaginationFilter? pagination = null)
    {
        return await Request<T>(HttpMethod.Get, url)
            .Authenticate()
            .WithPagination(pagination)
            .WithJson(requestContent)
            .Execute();
    }

    public async Task<ApiResponse<T>> GetUnauthenticated<T>(string url, Dictionary<string, string>? queryParameters = null, PaginationFilter? pagination = null)
    {
        var queryBuilder = Request<T>(HttpMethod.Get, url)
            .WithPagination(pagination);

        foreach (var (key, value) in queryParameters ?? [])
        {
            queryBuilder.AddQueryParameter(key, value);
        }

        return await queryBuilder.Execute();
    }

    public async Task<ApiResponse<T>> Put<T>(string url, object? requestContent = null)
    {
        return await Request<T>(HttpMethod.Put, url)
            .Authenticate()
            .WithJson(requestContent)
            .Execute();
    }

    public async Task<ApiResponse<T>> Patch<T>(string url, object? requestContent = null)
    {
        return await Request<T>(HttpMethod.Patch, url)
            .Authenticate()
            .WithJson(requestContent)
            .Execute();
    }

    public async Task<ApiResponse<T>> Delete<T>(string url, object? requestContent = null)
    {
        return await Request<T>(HttpMethod.Delete, url)
            .Authenticate()
            .WithJson(requestContent)
            .Execute();
    }

    public RequestBuilder<T> Request<T>(HttpMethod method, string url)
    {
        return new RequestBuilder<T>(this, _jsonSerializerOptions, _authenticator, method, url);
    }

    private async Task<ApiResponse<T>> Execute<T>(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStreamAsync();
        var statusCode = response.StatusCode;

        if (statusCode == HttpStatusCode.NoContent || responseContent.Length == 0)
        {
            responseContent.Close();
            responseContent = new MemoryStream(Encoding.UTF8.GetBytes(EMPTY_RESULT));
        }

        var deserializedResponseContent = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonSerializerOptions);

        deserializedResponseContent!.Status = statusCode;
        deserializedResponseContent.RawContent = await response.Content.ReadAsStringAsync();
        deserializedResponseContent.ContentType = response.Content.Headers.ContentType?.MediaType;

        return deserializedResponseContent;
    }

    private async Task<ApiResponse<T>> ExecuteOData<T>(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStreamAsync();
        var statusCode = response.StatusCode;

        if (statusCode == HttpStatusCode.NoContent || responseContent.Length == 0)
        {
            responseContent.Close();
            responseContent = new MemoryStream(Encoding.UTF8.GetBytes(EMPTY_VALUE));
        }

        var deserializedResponseContent = JsonSerializer.Deserialize<ODataResponse<T>>(responseContent, _jsonSerializerOptions)!;
        var rawContent = await response.Content.ReadAsStringAsync();
        deserializedResponseContent.ContentType = response.Content.Headers.ContentType?.MediaType;

        return deserializedResponseContent.ToApiResponse(statusCode, rawContent);
    }

    private async Task<RawApiResponse> ExecuteRaw(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var deserialized = (await response.Content.ReadFromJsonAsync<ApiResponse<EmptyResponse>>(_jsonSerializerOptions))!;
            return new RawApiResponse { Error = deserialized.Error, Status = response.StatusCode };
        }

        var responseBytes = await response.Content.ReadAsByteArrayAsync();
        return new RawApiResponse { Content = responseBytes, Status = response.StatusCode };
    }

    public class RequestBuilder<T>
    {
        private readonly IAuthenticator _authenticator;
        private readonly EndpointClient _client;
        private readonly NameValueCollection _extraHeaders = [];
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly HttpMethod _method;
        private readonly NameValueCollection _queryParameters = [];

        private readonly string _url;
        private bool _authenticated;
        private HttpContent _content;

        public RequestBuilder(EndpointClient client, JsonSerializerOptions jsonSerializerOptions, IAuthenticator authenticator, HttpMethod method, string url)
        {
            _client = client;
            _jsonSerializerOptions = jsonSerializerOptions;
            _authenticator = authenticator;

            _url = url;
            _method = method;
            _authenticated = false;
            _content = JsonContent.Create<object?>(null);
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
            if (pagination != null) AddQueryParameters(pagination);
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

        public RequestBuilder<T> AddQueryParameter(string key, IEnumerable<string> values)
        {
            foreach (var value in values)
                _queryParameters.Add(key, value);

            return this;
        }

        public RequestBuilder<T> AddQueryParameter(string key, object value)
        {
            _queryParameters.Add(key, JsonSerializer.SerializeToElement(value, _jsonSerializerOptions).ToString());
            return this;
        }

        public RequestBuilder<T> AddQueryParameters(IQueryParameterStorage parameters)
        {
            _queryParameters.Add(parameters.ToQueryParameters());
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

        public async Task<ApiResponse<T>> Execute()
        {
            return await _client.Execute<T>(await CreateRequestMessage());
        }

        public async Task<ApiResponse<T>> ExecuteOData()
        {
            return await _client.ExecuteOData<T>(await CreateRequestMessage());
        }

        public async Task<RawApiResponse> ExecuteRaw()
        {
            return await _client.ExecuteRaw(await CreateRequestMessage());
        }

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
