using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;
using Newtonsoft.Json;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
[Scope(Feature = "DELETE Clients")]
public class ClientsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private string _clientId;
    private HttpResponse<List<ClientDTO>>? _response;

    public ClientsStepDefinitions(ClientsApi clientsApi) : base()
    {
        _clientsApi = clientsApi;
        _clientId = string.Empty;
    }

    [Given(@"a Client c")]
    public async Task GivenAClientC()
    {
        var createClientRequest = new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = "a-client-display-name",
            ClientSecret = string.Empty
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.Content = JsonConvert.SerializeObject(createClientRequest);

        var response = await _clientsApi.CreateClient(requestConfiguration);

        var actualStatusCode = (int)response!.StatusCode;
        actualStatusCode.Should().Be(201);
        _clientId = response.Content!.Result!.ClientId;
    }

    [Given(@"a non-existent Client c")]
    public void GivenANonExistentClientC()
    {
        _clientId = "some-non-existent-client-id";
    }

    [When(@"a DELETE request is sent to the /Clients endpoint")]
    public async Task WhenADeleteRequestIsSentToTheClientsEndpoint()
    {
        _response = await _clientsApi.DeleteClient(_clientId, _requestConfiguration);
        _response.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Clients endpoint")]
    public async Task WhenAGetRequestIsSentToTheClientsEndpointAsync()
    {
        _response = await _clientsApi.GetAllClients(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Clients")]
    public void ThenTheResponseContainsAListOfClients()
    {
        _response!.Content!.Result.Should().NotBeNull();
        _response!.Content!.Result.Should().NotBeEmpty();
        _response.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Content.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }
}
