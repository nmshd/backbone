using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
public class IdentitiesApiStepDefinitions : BaseStepDefinitions<List<IdentitySummaryDTO>>
{
    private readonly IdentitiesApi _identitiesApi;

    public IdentitiesApiStepDefinitions(IdentitiesApi identitiesApi) : base(new HttpResponse<List<IdentitySummaryDTO>>())
    {
        _identitiesApi = identitiesApi;
    }


    [When(@"a GET request is sent to the Identities/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesEndpointAsync()
    {
        _response = await _identitiesApi.GetIdentities(_requestConfiguration);
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Identities")]
    public void ThenTheResponseContainsAList()
    {
        _response!.Content!.Result.Should().NotBeNull();
        _response!.Content!.Result.Should().NotBeEmpty();
    }
}
