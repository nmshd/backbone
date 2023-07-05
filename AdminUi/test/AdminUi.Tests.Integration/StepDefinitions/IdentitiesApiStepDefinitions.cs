using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Models;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
public class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse<List<IdentitySummaryDTO>>? _response;

    public IdentitiesApiStepDefinitions(IdentitiesApi identitiesApi) : base()
    {
        _identitiesApi = identitiesApi;
    }

    [When(@"a GET request is sent to the /Identities endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesEndpointAsync()
    {
        _response = await _identitiesApi.GetIdentities(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Identities")]
    public void ThenTheResponseContainsAList()
    {
        _response!.Content!.Result.Should().NotBeNull();
        _response!.Content!.Result.Should().NotBeEmpty();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}
