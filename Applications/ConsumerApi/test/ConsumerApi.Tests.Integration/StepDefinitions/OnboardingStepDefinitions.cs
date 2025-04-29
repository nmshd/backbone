using System.Text.RegularExpressions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class OnboardingStepDefinitions
{
    #region Constructor, Fields, Properties

    private HttpClient _client = null!;
    private HttpResponseMessage _onboardingResponse = null!;
    private readonly HttpClientFactory _httpClientFactory;

    public OnboardingStepDefinitions(HttpClientFactory httpClientFactory)
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

    [When($"a call is made to the /resource/resourceId endpoint for the {RegexFor.SINGLE_THING} app")]
    public async Task WhenACallIsMadeToTheRessourceResourceIdEndpointForTheEnmeshedApp(string appName)
    {
        var requestUrl = $"reference/tok12345?appName={appName}";
        _onboardingResponse = await _client.GetAsync(requestUrl, CancellationToken.None);
    }

    #endregion

    #region Then

    [Then($"the response contains a link containing {RegexFor.URL}")]
    public void ThenTheResponseContainsALinkContainingAppsAppleCom(string url)
    {
        _onboardingResponse.Should().NotBeNull();

        var responseContent = _onboardingResponse.Content.ReadAsStringAsync().Result;
        const string pattern = @"<a\s+(?:[^>]*?\s+)?href\s*=\s*[""']?(?<href>[^'"" >]+)[""']?";

        var matches = Regex.Matches(responseContent, pattern, RegexOptions.IgnoreCase);

        matches.Should().NotBeEmpty();
        matches.Should().Contain(link => link.Value.Contains(url));
    }

    [Then($"the response does not contain a link containing {RegexFor.URL}")]
    public void ThenTheResponseDoesNotContainALinkContainingPlayGoogleCom(string url)
    {
        _onboardingResponse.Should().NotBeNull();

        var responseContent = _onboardingResponse.Content.ReadAsStringAsync().Result;
        const string pattern = @"<a\s+(?:[^>]*?\s+)?href\s*=\s*[""']?(?<href>[^'"" >]+)[""']?";

        var matches = Regex.Matches(responseContent, pattern, RegexOptions.IgnoreCase);

        matches.Should().NotBeEmpty();
        matches.Should().NotContain(link => link.Value.Contains(url));
    }

    #endregion
}
