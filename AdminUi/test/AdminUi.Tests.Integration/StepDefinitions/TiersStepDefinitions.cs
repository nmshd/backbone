using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
public class TiersStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private HttpResponse<List<TierDTO>>? _response;

    public TiersStepDefinitions(TiersApi tiersApi) : base()
    {
        _tiersApi = tiersApi;
    }

    [When(@"a GET request is sent to the /Tiers endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpointAsync()
    {
        _response = await _tiersApi.GetTiers(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Tiers")]
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
