using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
public class TiersStepDefinitions : BaseStepDefinitions<List<TierDTO>>
{
    private readonly TiersApi _tiersApi;

    public TiersStepDefinitions(TiersApi tiersApi) : base(new HttpResponse<List<TierDTO>>())
    {
        _tiersApi = tiersApi;
    }

    [When(@"a GET request is sent to the Tiers/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpointAsync()
    {
        _response = await _tiersApi.GetTiers(_requestConfiguration);
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAList()
    {
        _response!.Content!.Result.Should().NotBeNull();
        _response!.Content!.Result.Should().NotBeEmpty();
    }
}
