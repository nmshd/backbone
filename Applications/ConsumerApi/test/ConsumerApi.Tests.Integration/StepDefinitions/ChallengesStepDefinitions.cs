using Backbone.ConsumerApi.Tests.Integration.Contexts;
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

    [Given($"a Challenge {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenAChallengeCreatedByIdentity(string challengeName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.ChallengeResponse = await client.Challenges.CreateChallenge();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }

    [Given($"a Challenge {RegexFor.SINGLE_THING} created by an anonymous user")]
    public async Task GivenAChallengeCreatedByAnAnonymousUser(string challengeName)
    {
        _responseContext.ChallengeResponse = await _clientPool.Anonymous.Challenges.CreateChallengeUnauthenticated();
        _responseContext.ChallengeResponse!.Should().BeASuccess();

        _challengesContext.Challenges[challengeName] = _responseContext.ChallengeResponse.Result!;
        _challengesContext.Challenges[challengeName].Id.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region When

    [When("an anonymous user sends a POST request is sent to the /Challenges endpoint")]
    public async Task WhenAnAnonymousUserSendsAPostRequestIsSentToTheChallengesEndpoint()
    {
        var client = _clientPool.Anonymous;

        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.CreateChallengeUnauthenticated();
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Challenges endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheChallengesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.CreateChallenge();
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Challenges/{{id}} endpoint with a valid id {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWithAValidId(string identityName, string challengeName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the Challenges/{{id}} endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWithChallengeId(string identityName, string challengeName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.GetChallenge(_challengesContext.Challenges[challengeName].Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the Challenges/{{id}} endpoint with \"{RegexFor.SINGLE_THING}\"")]
    public async Task WhenIdentitySendsAGetRequestToTheChallengesIdEndpointWith(string identityName, string challengeId)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _responseContext.ChallengeResponse = await client.Challenges.GetChallenge(challengeId);
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
