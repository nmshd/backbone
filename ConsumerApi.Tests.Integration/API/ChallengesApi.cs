using ConsumerApi.Tests.Integration.Models;
using RestSharp;

namespace ConsumerApi.Tests.Integration.API;

public class ChallengesApi : BaseApi
{
    public ChallengesApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<Challenge>> CreateChallenge(RequestConfiguration requestConfiguration)
    {
        return await Post<Challenge>("/challenges", requestConfiguration);
    }

    public async Task<HttpResponse<Challenge>> GetChallengeById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Challenge>($"/challenges/{id}", requestConfiguration);
    }
}
