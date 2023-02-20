using System.Text.Json;
using Challenges.API.Client.Interface;
using Challenges.API.Client.Models;
using IdentityModel.Client;

namespace Challenges.API.Client.API;
public class ChallengesClient : IChallengesClient
{
    // Use semaphore to prevent race conditions
    private static SemaphoreSlim _accessTokenSemaphore;

    private readonly string _baseUrl = "http://localhost:5000";
    private readonly string _tokenEndpoint = "/connect/token";

    private readonly HttpClient _httpClient;
    private static AccessTokenResponse _accessToken;

    public ChallengesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _accessToken = null;

        _accessTokenSemaphore = new SemaphoreSlim(1, 1);
    }

    public async Task<ChallengeResponse> CreateChallenge(AuthenticationRequest authenticationRequest)
    {
        if (authenticationRequest != null)
        {
            var tokenResponse = await GetAccessToken(authenticationRequest);
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
        }
        else
        {
            _httpClient.SetBearerToken(null);
        }

        var response = await _httpClient.PostAsync("/api/v1/challenges", null);

        var responseContent = await response.Content.ReadAsStringAsync();

        var challenge = new ChallengeResponse();

        if (response.IsSuccessStatusCode)
        {
            challenge = JsonSerializer.Deserialize<ChallengeResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        challenge.StatusCode = response.StatusCode;
        challenge.ReasonPhrase = response.ReasonPhrase;
        challenge.IsSuccessStatusCode = response.IsSuccessStatusCode;

        return challenge;
    }

    public async Task<ChallengeResponse> GetChallengeById(string challengeId, AuthenticationRequest authenticationRequest)
    {
        if (authenticationRequest != null)
        {
            var tokenResponse = await GetAccessToken(authenticationRequest);
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
        }
        else
        {
            _httpClient.SetBearerToken(null);
        }

        var response = await _httpClient.GetAsync($"/api/v1/challenges/{challengeId}");

        var responseContent = await response.Content.ReadAsStringAsync();

        var challenge = new ChallengeResponse();

        if (response.IsSuccessStatusCode)
        {
            challenge = JsonSerializer.Deserialize<ChallengeResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        challenge.StatusCode = response.StatusCode;
        challenge.ReasonPhrase = response.ReasonPhrase;
        challenge.IsSuccessStatusCode = response.IsSuccessStatusCode;

        return challenge;
    }

    public async Task<AccessTokenResponse> GetAccessToken(AuthenticationRequest authenticationRequest)
    {
        if (_accessToken is { Expired: false })
        {
            return _accessToken;
        }

        _accessToken = await FetchAccessToken(authenticationRequest);
        return _accessToken;
    }

    private async Task<AccessTokenResponse> FetchAccessToken(AuthenticationRequest authenticationRequest)
    {
        try
        {
            await _accessTokenSemaphore.WaitAsync();

            if (_accessToken is { Expired: false })
            {
                return _accessToken;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "content_type", "application/x-www-form-urlencoded" },
                { "grant_type", "password" },
                { "username", authenticationRequest.Username },
                { "password", authenticationRequest.Password }
            })
            };

            Console.WriteLine("Fetching Access Token...");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Console.WriteLine("Success!");

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccessTokenResponse>(responseContent);
        }
        finally
        {
            _accessTokenSemaphore.Release(1);
        }
    }
}
