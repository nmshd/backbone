using System.Net;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class OnboardingStepDefinition
{
    #region Constructor, Fields, Properties

    private Client _client = null!;
    private ApiResponse<EmptyResponse> _onboardingResponse = null!;
    private readonly ResponseContext _responseContext;
    private readonly HttpConfiguration _configuration;
    private readonly HttpClientFactory _httpClientFactory;
    private readonly ClientPool _clientPool;

    public OnboardingStepDefinition(IOptions<HttpConfiguration> configuration, HttpClientFactory httpClientFactory, ResponseContext responseContext, ClientPool clientPool)
    {
        _httpClientFactory = httpClientFactory;
        _responseContext = responseContext;
        _clientPool = clientPool;
        _configuration = configuration.Value;
    }

    #endregion

    #region Given

    [Given($@"an anonymous client with user agent {RegexFor.SINGLE_THING}")]
    public void GivenAnAnonymousUser(string userAgentContent)
    {
        var clientCredentials = new ClientCredentials(_configuration.ClientCredentials.ClientId, _configuration.ClientCredentials.ClientSecret);
        var httpClient = _httpClientFactory.CreateClient();

        //httpClient.DefaultRequestHeaders.UserAgent.("User-Agent", userAgentContent);
        _client = Client.CreateUnauthenticated(httpClient, clientCredentials);
    }

    #endregion

    #region When

    [When("a call is made to the /Onboarding endpoint")]
    public async void WhenACallIsMadeToTheOnboardingEndpoint()
    {
        _client = _clientPool.Anonymous;
        _onboardingResponse = await _client.Onboarding.RequestOnboardingInformation();
    }

    #endregion

    #region Then

    [Then(@$"the response type is {"([a-zA-Z0-9/]+)"}")]
    public void ThenTheResponseTypeIs(string contentType)
    {
        _onboardingResponse.ContentType.Should().NotBeNullOrWhiteSpace();
        _onboardingResponse.ContentType.Should().Be(contentType);
    }

    [Then(@$"the redirection location contains {RegexFor.SINGLE_THING}")]
    public void ThenTheRedirectionLocationContains(string redirectionUrl)
    {
        _onboardingResponse.Result.Should().NotBeNull();
        _onboardingResponse.Status.Should().Be(HttpStatusCode.Redirect);
        _onboardingResponse.RawContent.Should().Contain(redirectionUrl);
    }

    #endregion
}
