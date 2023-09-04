using ConsumerApi.Tests.Integration.Models;

namespace ConsumerApi.Tests.Integration.API;

public class ChallengesApi : BaseApi
{
    public ChallengesApi(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    public async Task<HttpResponse<Challenge>> CreateChallenge(RequestConfiguration requestConfiguration)
    {
        return await Post<Challenge>("/Challenges", requestConfiguration);
    }

    public async Task<HttpResponse<Challenge>> GetChallengeById(RequestConfiguration requestConfiguration, string id)
    {
        return await Get<Challenge>($"/Challenges/{id}", requestConfiguration);
    }
}
