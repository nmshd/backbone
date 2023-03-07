using System.Net;
using Challenges.API.Tests.Integration.Models;
using RestSharp;

namespace Challenges.API.Tests.Integration.API;
public class ChallengesApi
{
    private readonly RestClient _client;

    public ChallengesApi(RestClient client)
    {
        _client = client;

        ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
    }

    public async Task<RestResponse<AccessTokenResponse>> GetAccessTokenAsync(AuthenticationParameters authenticationParams)
    {
        var request = new RestRequest("/connect/token", Method.Post);

        //RestSharp will use the correct content type by default (https://restsharp.dev/usage.html#http-header)
        //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        foreach (var parameter in authenticationParams.Parameters)
        {
            request.AddParameter(parameter.Key, parameter.Value);
        }

        return await _client.ExecuteAsync<AccessTokenResponse>(request);
    }

    public async Task<RestResponse<ChallengeResponse>> CreateChallengeAsync(RequestConfiguration requestConfiguration)
    {
        return await ExecuteChallengeRequestAsync(Method.Post, "/api/v1/challenges", requestConfiguration);
    }

    public async Task<RestResponse<ChallengeResponse>> GetChallengeByIdAsync(RequestConfiguration requestConfiguration, string id)
    {
        return await ExecuteChallengeRequestAsync(Method.Get, $"/api/v1/challenges/{id}", requestConfiguration);
    }

    private async Task<RestResponse<ChallengeResponse>> ExecuteChallengeRequestAsync(Method method, string endpoint, RequestConfiguration requestConfiguration)
    {
        var request = new RestRequest(endpoint, method);

        if (!string.IsNullOrEmpty(requestConfiguration.Body))
            request.AddBody(requestConfiguration.Body);

        if (!string.IsNullOrEmpty(requestConfiguration.ContentType))
            request.AddHeader("Content-Type", requestConfiguration.ContentType);

        if (!string.IsNullOrEmpty(requestConfiguration.AcceptHeader))
            request.AddHeader("Accept", requestConfiguration.AcceptHeader);

        if (!string.IsNullOrEmpty(requestConfiguration.TokenResponse?.AccessToken))
            request.AddHeader("Authorization", $"Bearer {requestConfiguration.TokenResponse.AccessToken}");

        return await _client.ExecuteAsync<ChallengeResponse>(request);
    }
}
