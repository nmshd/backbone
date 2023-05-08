using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;
using AdminApi.Tests.Integration.Utils.Models;
using AdminApi.Tests.Integration.Utils.StepDefinitions;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "Get Identity List")]
public class GetIdentityListStepDefinitions : BaseStepDefinitions<List<IdentitySummaryDTO>>
{
    private readonly IdentitiesApi _identitiesApi;
    private List<IdentitySummaryDTO>? _identitiesList;

    public GetIdentityListStepDefinitions(IdentitiesApi identitiesApi) : base(new HttpResponse<List<IdentitySummaryDTO>>())
    {
        _identitiesApi = identitiesApi;
    }


    [When(@"a GET request is sent to the Identities/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesEndpointAsync()
    {
        _response = await _identitiesApi.GetIdentities(_requestConfiguration);
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
        _response.StatusCode = _response!.StatusCode;
        _identitiesList = _response!.Content!.Result;
    }

    [Then(@"the response contains a paginated list of Identities")]
    public void ThenTheResponseContainsAList()
    {
        _identitiesList.Should().NotBeNull();
        _identitiesList!.Should().NotBeEmpty();
    }
}
