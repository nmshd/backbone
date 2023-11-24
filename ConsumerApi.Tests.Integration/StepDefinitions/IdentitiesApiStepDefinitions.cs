using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identity")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private HttpResponse<CreateIdentityResponse>? _identityResponse;
    private HttpResponse<Challenge>? _challengeResponse;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi, ChallengesApi challengesApi, ISignatureHelper signatureHelper) : 
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi) { }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        await CreateChallenge();
    }

    [When(@"a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        _identityResponse = await CreateIdentity();
    }

    [Given(@"an Identity i")]
    public async Task GivenAnIdentityI()
    {
        _challengeResponse = await CreateChallenge();
        _identityResponse = await CreateIdentity();
    }

    [Then(@"the response contains a CreateIdentityResponse")]
    public void ThenTheResponseContainsACreateIdentityResponse()
    {
        _identityResponse!.Should().NotBeNull();
        _identityResponse!.IsSuccessStatusCode.Should().BeTrue();
        _identityResponse!.ContentType.Should().Be("application/json");
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_identityResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}
