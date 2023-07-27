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
    private HttpResponse<List<IdentitySummaryDTO>>? _identitiesResponse;
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
    public async Task WhenAGETRequestIsSentToTheIdentitiesEndpoint()
    {
        _identitiesResponse = await _identitiesApi.GetIdentities(_requestConfiguration);
        _identitiesResponse.Should().NotBeNull();
        _identitiesResponse.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/{i.address} endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpoint()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, _existingIdentity);
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/inexistentIdentityAddress endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpointForAnInexistentIdentity()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, "inexistentIdentityAddress");
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Identities")]
    public void ThenTheResponseContainsAList()
    {
        _identitiesResponse!.Content.Result.Should().NotBeNull();
        _identitiesResponse!.Content.Result.Should().NotBeEmpty();
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

        if (_identitiesResponse != null)
        {
            var actualStatusCode = (int)_identitiesResponse!.StatusCode;
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

        if (_identitiesResponse != null)
        {
            _identityResponse!.Content.Error.Should().NotBeNull();
            _identityResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}
