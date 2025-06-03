using System.Text.RegularExpressions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class AppOnboardingStepDefinitions
{
    #region Constructor, Fields, Properties

    private HttpClient _client = null!;
    private HttpResponseMessage _onboardingResponse = null!;
    private readonly HttpClientFactory _httpClientFactory;

    public AppOnboardingStepDefinitions(HttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    #endregion

    #region Given

    [Given($"an http client with user agent \"{RegexFor.SINGLE_THING}\"")]
    public void GivenAHttpClient(string userAgentContent)
    {
        _client = _httpClientFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("User-Agent", userAgentContent);
    }

    #endregion

    #region When

    [When($"^a call is made to the /r/resourceId endpoint for the app \"{RegexFor.SINGLE_THING}\"$")]
    public async Task WhenACallIsMadeToTheRResourceIdEndpointFor(string appId)
    {
        var requestUrl = $"r/tok12345?app={appId}";
        _onboardingResponse = await _client.GetAsync(requestUrl, CancellationToken.None);
    }

    #endregion

    #region Then

    [Then($"the response contains a link with the domain name \"{RegexFor.URL}\"")]
    public void ThenTheResponseContainsALinkContaining(string url)
    {
        _onboardingResponse.ShouldNotBeNull();

        var responseContent = _onboardingResponse.Content.ReadAsStringAsync().Result;
        const string pattern = @"<a\s+(?:[^>]*?\s+)?href\s*=\s*[""']?(?<href>[^'"" >]+)[""']?";

        var matches = Regex.Matches(responseContent, pattern, RegexOptions.IgnoreCase);

        matches.ShouldNotBeEmpty();
        matches.ShouldContain(link => link.Value.Contains(url));
    }

    [Then($"the response does not contains a link with the domain name \"{RegexFor.URL}\"")]
    public void ThenTheResponseDoesNotContainALinkContaining(string url)
    {
        _onboardingResponse.ShouldNotBeNull();

        var responseContent = _onboardingResponse.Content.ReadAsStringAsync().Result;
        const string pattern = @"<a\s+(?:[^>]*?\s+)?href\s*=\s*[""']?(?<href>[^'"" >]+)[""']?";

        var matches = Regex.Matches(responseContent, pattern, RegexOptions.IgnoreCase);

        matches.ShouldNotBeEmpty();
        matches.ShouldNotContain(link => link.Value.Contains(url));
    }

    #endregion
}
