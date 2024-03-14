using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Challenges;

public class ChallengesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<Challenge>> CreateChallenge() => await _client.Post<Challenge>("Challenges");
    public async Task<ConsumerApiResponse<Challenge>> CreateChallengeUnauthenticated() => await _client.PostUnauthenticated<Challenge>("Challenges");

    public async Task<ConsumerApiResponse<Challenge>> GetChallenge(string id) => await _client.Get<Challenge>($"Challenges/{id}");
}
