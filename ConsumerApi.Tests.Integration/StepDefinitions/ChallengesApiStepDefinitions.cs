using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ChallengesApiStepDefinitions
{
    private readonly ChallengesContext _challengesContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public ChallengesApiStepDefinitions(ChallengesContext challengesContext, IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _challengesContext = challengesContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private Client AnonymousClient => _identitiesContext.AnonymousClient!;
    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    private string ChallengeId => _challengesContext.ChallengeId!;
    private ApiResponse<Challenge> ChallengeResponse => _responseContext.ChallengeResponse!;

    #region Given
    [Given("a Challenge c created by (.+)")]
    public async Task GivenAChallengeCreatedByIdentity(string identityName)
    {
        _responseContext.ChallengeResponse = _challengesContext.IsAuthenticated ? 
            await Identity(identityName).Challenges.CreateChallenge() : 
            await Identity(identityName).Challenges.CreateChallengeUnauthenticated();
        ChallengeResponse.Should().BeASuccess();

        _challengesContext.ChallengeId = ChallengeResponse.Result!.Id;
        ChallengeId.Should().NotBeNullOrEmpty();
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        _responseContext.ChallengeResponse = _challengesContext.IsAuthenticated ?
            await AnonymousClient.Challenges.CreateChallenge() :
            await AnonymousClient.Challenges.CreateChallengeUnauthenticated();
        ChallengeResponse.Should().BeASuccess();

        _challengesContext.ChallengeId = ChallengeResponse.Result!.Id;
        ChallengeId.Should().NotBeNullOrEmpty();
    }
    #endregion

    #region When
    [When("a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = _challengesContext.IsAuthenticated ? 
            await AnonymousClient.Challenges.CreateChallenge() : 
            await AnonymousClient.Challenges.CreateChallengeUnauthenticated();
    }

    [When(@"(.+) sends a POST request to the /Challenges endpoint")]
    public async Task WhenISendsAPOSTRequestToTheChallengesEndpoint(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = _challengesContext.IsAuthenticated ?
            await Identity(identityName).Challenges.CreateChallenge() :
            await Identity(identityName).Challenges.CreateChallengeUnauthenticated();
    }

    [When(@"(.+) sends a GET request is sent to the Challenges/{id} endpoint with ""?(.*?)""?")]
    public async Task WhenAGETRequestIsSentToTheChallengesIdEndpointWith(string identityName, string challengeId)
    {
        switch (challengeId)
        {
            case "c.Id":
                challengeId = _challengesContext.ChallengeId!;
                break;
            case "a valid Id":
                challengeId = VALID_CHALLENGE_ID;
                break;
        }

        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await Identity(identityName).Challenges.GetChallenge(challengeId);
    }
    #endregion

    #region Then
    [Then("the Challenge does not contain information about the creator")]
    public void ThenTheChallengeDoesNotContainInformationAboutTheCreator()
    {
        ChallengeResponse.Result!.CreatedBy.Should().BeNull();
        ChallengeResponse.Result!.CreatedByDevice.Should().BeNull();
    }

    [Then("the Challenge contains information about the creator")]
    public void ThenTheChallengeContainsInformationAboutTheCreator()
    {
        ChallengeResponse.Result!.CreatedBy.Should().NotBeNull();
        ChallengeResponse.Result!.CreatedByDevice.Should().NotBeNull();
    }
    #endregion
}

public class ChallengesContext
{
    public string? ChallengeId { get; set; }
    public bool IsAuthenticated { get; set; }
}
