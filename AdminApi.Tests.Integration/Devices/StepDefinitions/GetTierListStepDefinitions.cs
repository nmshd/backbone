using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;
using AdminApi.Tests.Integration.Utils.Models;
using AdminApi.Tests.Integration.Utils.StepDefinitions;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "Get Tier List")]
public class GetTierListStepDefinitions : BaseStepDefinitions<List<TierDTO>>
{
    private readonly TiersApi _tiersApi;
    private List<TierDTO>? _tiersList;

    public GetTierListStepDefinitions(TiersApi tiersApi) : base(new HttpResponse<List<TierDTO>>())
    {
        _tiersApi = tiersApi;
    }

    [When(@"a GET request is sent to the Tiers/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpointAsync()
    {
        _response = await _tiersApi.GetTiers(_requestConfiguration);
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
        _response.StatusCode = _response!.StatusCode;
        _tiersList = _response!.Content!.Result;
    }

    [Then(@"the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAList()
    {
        _tiersList.Should().NotBeNull();
        _tiersList!.Should().NotBeEmpty();
    }
}
