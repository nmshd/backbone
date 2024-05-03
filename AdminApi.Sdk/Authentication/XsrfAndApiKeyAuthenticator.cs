using System.Diagnostics.CodeAnalysis;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;

namespace Backbone.AdminApi.Sdk.Endpoints.Common;

public class XsrfAndApiKeyAuthenticator : IAuthenticator
{
    private readonly string _apiKey;
    private string? _xsrfToken = null;
    private string? _xsrfCookie = null;
    private readonly HttpClient _client;

    public XsrfAndApiKeyAuthenticator(string apiKey, HttpClient client)
    {
        _apiKey = apiKey;
        _client = client;
    }

    public async Task Authenticate(HttpRequestMessage request)
    {
        request.Headers.Add("X-API-KEY", _apiKey);
        request.Headers.Add("X-XSRF-TOKEN", await GetToken());
        request.Headers.Add("Cookie", await GetCookie());
    }

    private async Task<string> GetToken()
    {
        if (_xsrfToken == null) await RefreshToken();

        return _xsrfToken;
    }

    private async Task<string> GetCookie()
    {
        if (_xsrfCookie == null) await RefreshToken();

        return _xsrfCookie;
    }

    [MemberNotNull(nameof(_xsrfToken), nameof(_xsrfCookie))]
    private async Task RefreshToken()
    {
        HttpRequestMessage request = new(HttpMethod.Get, "api/v1/xsrf");
        request.Headers.Add("X-API-KEY", _apiKey);

#pragma warning disable CS8774 // This warning ("Member must have a non-null value when exiting") must currently be disabled. (see https://github.com/dotnet/csharplang/discussions/ for details)
        var response = await _client.SendAsync(request);

        _xsrfToken = await response.Content.ReadAsStringAsync();
        _xsrfCookie = response.Headers.GetValues("Set-Cookie").First();
#pragma warning restore CS8774
    }
}
