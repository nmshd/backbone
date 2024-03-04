using Backbone.ConsumerApi.Tests.Integration.Models;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class ChallengesApi : BaseApi
{
    public ChallengesApi(HttpClientFactory factory) : base(factory) { }

    internal async Task<HttpResponse<Challenge>> CreateChallenge(RequestConfiguration requestConfiguration)
    {
        return await Post<Challenge>("/Challenges", requestConfiguration);
    }

    internal async Task<HttpResponse<Challenge>> GetChallengeById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Challenge>($"/Challenges/{id}", requestConfiguration);
    }
}
