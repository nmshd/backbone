using ConsumerApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace ConsumerApi.Tests.Integration.API;

public class ChallengesApi : BaseApi
{
    public ChallengesApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<Challenge>> CreateChallenge(RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<Challenge>(Method.Post, new PathString(ROUTE_PREFIX).Add("/challenges").ToString(), requestConfiguration);
    }

    public async Task<HttpResponse<Challenge>> GetChallengeById(RequestConfiguration requestConfiguration, string id)
    {
        return await ExecuteRequest<Challenge>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/challenges/{id}").ToString(), requestConfiguration);
    }
}
