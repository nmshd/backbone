using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
public class ClientsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private HttpResponse<List<ClientDTO>>? _response;

    public ClientsStepDefinitions(ClientsApi clientsApi) : base()
    {
        _clientsApi = clientsApi;
    }

    [When(@"a GET request is sent to the Clients/ endpoint")]
    public async Task WhenAGETRequestIsSentToTheClientsEndpointAsync()
    {
        _response = await _clientsApi.GetAllClients(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Clients")]
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
