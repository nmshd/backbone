using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ChallengesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ChallengesContext _challengesContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public ChallengesStepDefinitions(ChallengesContext challengesContext, ResponseContext responseContext, ClientPool clientPool)
    {
        _challengesContext = challengesContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given("a Challenge ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenAChallengeCreatedByIdentity(string challengeName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.ChallengeResponse = await client.Challenges.CreateChallenge();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }

    [Given(@"a Challenge ([a-zA-Z0-9]+)")]
    public async Task GivenAChallenge(string challengeName)
    {
        var client = _clientPool.Default();
        _responseContext.ChallengeResponse = await client!.Challenges.CreateChallengeUnauthenticated();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region When

    [When("a POST request is sent to the /Challenges endpoint")]
    public async Task WhenAPostRequestIsSentToTheChallengesEndpoint()
    {
        var client = _clientPool.Default();

        _responseContext.WhenResponse = _responseContext.ChallengeResponse =
            _clientPool.IsDefaultClientAuthenticated() ? await client!.Challenges.CreateChallenge() : await client!.Challenges.CreateChallengeUnauthenticated();
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Challenges endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheChallengesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.CreateChallenge();
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Challenges/{id} endpoint with a valid id ([a-zA-Z0-9]+)\.Id")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWithAValidId(string identityName, string challengeName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Challenges/{id} endpoint with a placeholder id ""?(.*?)""?")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWithAPlaceholderId(string identityName, string challengeId)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.GetChallenge(challengeId);
    }

    [When(@"i sends a GET request to the Challenges/\{id} endpoint with ([a-zA-Z0-9]+)\.Id")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWithChallengeId(string challengeName)
    {
        var client = _clientPool.FirstForDefaultIdentity();
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client!.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When(@"i sends a GET request to the Challenges/\{id} endpoint with \""([a-zA-Z0-9]+)\""")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWith(string challengeId)
    {
        var client = _clientPool.FirstForDefaultIdentity();
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client!.Challenges.GetChallenge(challengeId);
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
