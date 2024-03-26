using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Challenges;

public class ChallengesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<Challenge>> CreateChallenge() => await _client.Post<Challenge>("Challenges");
    public async Task<ApiResponse<Challenge>> CreateChallengeUnauthenticated() => await _client.PostUnauthenticated<Challenge>("Challenges");

    public async Task<ApiResponse<Challenge>> GetChallenge(string id) => await _client.Get<Challenge>($"Challenges/{id}");
}
