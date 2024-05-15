using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Challenges;

public class ChallengesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<Challenge>> CreateChallenge()
    {
        return await _client.Post<Challenge>("Challenges");
    }

    public async Task<ApiResponse<Challenge>> CreateChallengeUnauthenticated()
    {
        return await _client.PostUnauthenticated<Challenge>("Challenges");
    }
    
    public async Task<ApiResponse<Challenge>> GetChallenge(string id)
    {
        return await _client.Get<Challenge>($"Challenges/{id}");
    }

    public async Task<ApiResponse<Challenge>> GetChallengeUnauthenticated(string id)
    {
        return  await _client.GetUnauthenticated<Challenge>($"api/{API_VERSION}/Challenges/{id}");
    }
}
