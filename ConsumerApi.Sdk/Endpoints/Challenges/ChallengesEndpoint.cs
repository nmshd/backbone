using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Challenges;

public class ChallengesEndpoint
{
    private readonly EndpointClient _client;

    public ChallengesEndpoint(EndpointClient client)
    {
        _client = client;
    }

    public async Task<ConsumerApiResponse<Challenge>> CreateChallenge()
    {
        return await _client.Post<Challenge>("Challenges");
    }

    public async Task<ConsumerApiResponse<Challenge>> CreateChallengeUnauthenticated()
    {
        return await _client.PostUnauthenticated<Challenge>("Challenges");
    }
}
