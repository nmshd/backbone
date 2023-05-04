using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
public class GetTierListStepDefinitions
{
    private readonly ResponseData _responseData;
    private readonly RequestConfiguration _requestConfiguration;
    private readonly TiersApi _tiersApi;
    private HttpResponse<ListTiersResponse>? _tiersResponse;
    private List<TierDTO>? _tiersList;

    public GetTierListStepDefinitions(TiersApi tiersApi, ResponseData responseData)
    {
        _responseData = responseData;
        _tiersApi = tiersApi;
        _requestConfiguration = new RequestConfiguration();
    }


    [When(@"a GET request is sent to the Tiers/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpointAsync()
    {
        _tiersResponse = await _tiersApi.GetTiers(_requestConfiguration);
        _tiersResponse.Should().NotBeNull();
        _tiersResponse!.Data.Should().NotBeNull();
        _tiersList = _tiersResponse!.Data!.Result;

        _responseData.ResponseStatus = _tiersResponse!.StatusCode;
    }

    [Then(@"the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAList()
    {
        _tiersList.Should().NotBeNull();
        _tiersList!.Should().NotBeEmpty();
    }
}
