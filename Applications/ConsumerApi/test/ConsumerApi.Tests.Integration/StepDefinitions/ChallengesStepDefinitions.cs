using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Integration.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ChallengesStepDefinitions
{
    #region Constructor, Fields, Properties
    private readonly ChallengesContext _challengesContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public ChallengesStepDefinitions(ChallengesContext challengesContext, IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _challengesContext = challengesContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;
    #endregion

    #region Given
    [Given("a Challenge ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenAChallengeCreatedByI(string challengeName, string identityName)
    {
        _responseContext.ChallengeResponse = await ClientPool.FirstForIdentity(identityName)!.Challenges.CreateChallenge();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }

    [Given(@"a Challenge ([a-zA-Z0-9]+)")]
    public async Task GivenAChallengeC(string challengeName)
    {
        _responseContext.ChallengeResponse = await ClientPool.Default()!.Challenges.CreateChallengeUnauthenticated();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }
    #endregion

    #region When
    [When("a POST request is sent to the /Challenges endpoint")]
    public async Task WhenAPostRequestIsSentToTheChallengesEndpoint()
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = ClientPool.IsDefaultClientAuthenticated() ?
            await ClientPool.Default()!.Challenges.CreateChallenge() :
            await ClientPool.Default()!.Challenges.CreateChallengeUnauthenticated();
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Challenges endpoint")]
    public async Task WhenISendsAPostRequestToTheChallengesEndpoint(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await ClientPool.FirstForIdentity(identityName)!.Challenges.CreateChallenge();
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Challenges/{id} endpoint with a valid id ([a-zA-Z0-9]+)\.Id")]
    public async Task WhenISendsAGetRequestToTheChallengesIdEndpointWithAValidId(string identityName, string challengeName)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await ClientPool.FirstForIdentity(identityName)!.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Challenges/{id} endpoint with a placeholder id ""?(.*?)""?")]
    public async Task WhenISendsAGetRequestToTheChallengesIdEndpointWith(string identityName, string challengeId)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await ClientPool.FirstForIdentity(identityName)!.Challenges.GetChallenge(challengeId);
    }

    [When(@"i sends a GET request to the Challenges/\{id} endpoint with ([a-zA-Z0-9]+)\.Id")]
    public async Task WhenISendsAGetRequestToTheChallengesIdEndpointWithChallengeId(string challengeName)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await ClientPool.FirstForDefaultIdentity()!.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When(@"i sends a GET request to the Challenges/\{id} endpoint with \""([a-zA-Z0-9]+)\""")]
    public async Task WhenAGetRequestIsSentToTheChallengesIdEndpointWithString(string challengeId)
    {
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await ClientPool.FirstForDefaultIdentity()!.Challenges.GetChallenge(challengeId);
    }
    #endregion

    #region Then
    [Then("the Challenge does not contain information about the creator")]
    public void ThenTheChallengeDoesNotContainInformationAboutTheCreator()
    {
        _responseContext.ChallengeResponse!.Result!.CreatedBy.Should().BeNull();
        _responseContext.ChallengeResponse!.Result!.CreatedByDevice.Should().BeNull();
    }

    [Then("the Challenge contains information about the creator")]
    public void ThenTheChallengeContainsInformationAboutTheCreator()
    {
        _responseContext.ChallengeResponse!.Result!.CreatedBy.Should().NotBeNull();
        _responseContext.ChallengeResponse!.Result!.CreatedByDevice.Should().NotBeNull();
    }
    #endregion
}

public class ChallengesContext
{
    public readonly Dictionary<string, Challenge> Challenges = new();
}
