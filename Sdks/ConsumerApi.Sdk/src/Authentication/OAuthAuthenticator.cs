using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.Tooling;

namespace Backbone.ConsumerApi.Sdk.Authentication;

public class OAuthAuthenticator : IAuthenticator
{
    private static readonly JsonSerializerOptions SERIALIZER_OPTIONS = new() { PropertyNameCaseInsensitive = true };

    private readonly Configuration.AuthenticationConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _jwtRequestData;
    private Jwt? _jwt;

    public OAuthAuthenticator(Configuration.AuthenticationConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;

        _jwtRequestData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", config.UserCredentials?.Username! },
            { "password", config.UserCredentials?.Password! },
            { "client_id", config.ClientCredentials.ClientId },
            { "client_secret", config.ClientCredentials.ClientSecret }
        };
    }

    public async Task Authenticate(HttpRequestMessage request)
    {
        if (_config.UserCredentials == null)
            throw new InvalidOperationException("User credentials are required in order to authenticate this request.");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwt());
    }

    private async Task<string> GetJwt()
    {
        if (_jwt == null || _jwt.IsExpired)
            await RefreshToken();

        return _jwt.AccessToken;
    }

    [MemberNotNull(nameof(_jwt))]
    private async Task RefreshToken()
    {
        var requestContent = new FormUrlEncodedContent(_jwtRequestData);
        var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token") { Content = requestContent };

#pragma warning disable CS8774 // This warning ("Member must have a non-null value when exiting") must currently be disabled. (see https://github.com/dotnet/csharplang/discussions/ for details)
        var httpResponse = await _httpClient.SendAsync(request).ConfigureAwait(false);
#pragma warning restore CS8774

#pragma warning disable CS8774 // This warning ("Member must have a non-null value when exiting") must currently be disabled. (see https://github.com/dotnet/csharplang/discussions/ for details)
        var accessTokenResponse = (await httpResponse.Content.ReadFromJsonAsync<AccessTokenResponse>(SERIALIZER_OPTIONS))!;
#pragma warning restore CS8774

        _jwt = new Jwt(accessTokenResponse.AccessToken, accessTokenResponse.ExpiresIn);
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class AccessTokenResponse
    {
        public AccessTokenResponse(string accessToken, int expiresIn)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
        }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; }
    }

    private class Jwt
    {
        private readonly DateTime _expiresAt;

        public Jwt(string accessToken, int expiresIn)
        {
            AccessToken = accessToken;
            _expiresAt = SystemTime.UtcNow + TimeSpan.FromSeconds(expiresIn);
        }

        public string AccessToken { get; }

        // we consider JWTs expired 30 seconds before they actually are
        public bool IsExpired => SystemTime.UtcNow > _expiresAt - TimeSpan.FromSeconds(30);
    }
}
