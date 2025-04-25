using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class OnboardingStepDefinitions
{
    #region Constructor, Fields, Properties

    private HttpClient _client = null!;
    private HttpResponseMessage _onboardingResponse = null!;
    private readonly HttpClientFactory _httpClientFactory;

    public OnboardingStepDefinitions(IOptions<HttpConfiguration> configuration, HttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    #endregion

    #region Given

    [Given($"a http client with user agent {RegexFor.SINGLE_THING}")]
    public void GivenAHttpClient(string userAgentContent)
    {
        _client = _httpClientFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("User-Agent", userAgentContent);
    }

    #endregion

    #region When

    [When($"a call is made to the /Tokens endpoint")]
    public async Task WhenACallIsMadeToTheOnboardingEndpoint()
    {
        const string requestUrl = "Tokens/tok12345";
        _onboardingResponse = await _client.GetAsync(requestUrl, CancellationToken.None);
    }

    #endregion

    #region Then

    [Then(@$"the response type is {"([a-zA-Z0-9/]+)"}")]
    public void ThenTheResponseTypeIs(string contentType)
    {
        _onboardingResponse.Should().NotBeNull();
        _onboardingResponse.Content.Headers.ContentType.Should().NotBeNull();
        _onboardingResponse.Content.Headers.ContentType!.MediaType.Should().BeEquivalentTo(contentType);
    }

    [Then(@$"the redirection location contains {RegexFor.URL}")]
    public void ThenTheRedirectionLocationContains(string redirectionUrl)
    {
        _onboardingResponse.Should().NotBeNull();
        _onboardingResponse.Headers.Location.Should().NotBeNull();
        _onboardingResponse.Headers.Location!.ToString().Should().Contain(redirectionUrl);
    }

    #endregion
}
