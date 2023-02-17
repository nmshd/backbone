using System.Net;
using Challenges.API.Client.Models;
using RestSharp;

namespace Challenges.API.Client.API;
public class ChallengesClient
{
    private readonly RestClient _client;

    public ChallengesClient()
    {
        _client = new RestClient("http://localhost:5000");

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<RestResponse<AccessTokenResponse>> GetAccessTokenAsync(AuthenticationRequest authenticationRequest)
    {
        var request = new RestRequest("/connect/token", Method.Post);
        request.AddParameter("grant_type", "password");
        request.AddParameter("client_id", authenticationRequest.ClientId);
        request.AddParameter("client_secret", authenticationRequest.ClientSecret);
        request.AddParameter("username", authenticationRequest.Username);
        request.AddParameter("password", authenticationRequest.Password);

        return await _client.ExecuteAsync<AccessTokenResponse>(request);
    }

    public async Task<RestResponse<ChallengeResponse>> CreateChallengeAsync(string accessToken = default)
    {
        var request = new RestRequest("/api/v1/challenges", Method.Post);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.AddHeader("Authorization", $"Bearer {accessToken}");
        }

        return await _client.ExecuteAsync<ChallengeResponse>(request);
    }

    public async Task<RestResponse<ChallengeResponse>> GetChallengeByIdAsync(string id, string accessToken = default)
    {
        var request = new RestRequest($"/api/v1/challenges/{id}", Method.Get);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.AddHeader("Authorization", $"Bearer {accessToken}");
        }

        return await _client.ExecuteGetAsync<ChallengeResponse>(request);
    }
}
