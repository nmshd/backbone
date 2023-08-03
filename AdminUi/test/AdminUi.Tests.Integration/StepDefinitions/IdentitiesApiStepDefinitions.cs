using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;
using AdminUi.Tests.Integration.TestData;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
public class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse<List<IdentityOverviewDTO>>? _identityOverviewsResponse;
    private HttpResponse<IdentitySummaryDTO>? _identityResponse;
    private string _existingIdentity;

    public IdentitiesApiStepDefinitions(IdentitiesApi identitiesApi)
    {
        _identitiesApi = identitiesApi;
        _existingIdentity = string.Empty;
    }

    [Given(@"an Identity i")]
    public void GivenAnIdentity()
    {
        _existingIdentity = Identities.IDENTITY_A;
    }

    [When(@"a GET request is sent to the /Identities endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesOverviewEndpoint()
    {
        _identityOverviewsResponse = await _identitiesApi.GetIdentityOverviews(_requestConfiguration);
        _identityOverviewsResponse.Should().NotBeNull();
        _identityOverviewsResponse!.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/{i.address} endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpoint()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, _existingIdentity);
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/{address} endpoint with an inexistent address")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpointForAnInexistentIdentity()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, "inexistentIdentityAddress");
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a list of Identities")]
    public void ThenTheResponseContainsAListOfIdentities()
    {
        _identityOverviewsResponse!.Content.Result.Should().NotBeNull();
        _identityOverviewsResponse!.Content.Result.Should().NotBeNullOrEmpty();
        _identityOverviewsResponse!.AssertContentTypeIs("application/json");
        _identityOverviewsResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains Identity i")]
    public void ThenTheResponseContainsAnIdentity()
    {
        _identityResponse!.AssertHasValue();
        _identityResponse!.AssertStatusCodeIsSuccess();
        _identityResponse!.AssertContentTypeIs("application/json");
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_identityResponse != null)
        {
            var actualStatusCode = (int)_identityResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_identityOverviewsResponse != null)
        {
            var actualStatusCode = (int)_identityOverviewsResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_identityResponse != null)
        {
            _identityResponse!.Content.Error.Should().NotBeNull();
            _identityResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}
