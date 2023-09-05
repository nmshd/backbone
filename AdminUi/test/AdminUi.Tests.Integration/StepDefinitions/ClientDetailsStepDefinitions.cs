using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;

namespace AdminUi.Tests.Integration.StepDefinitions;
public class ClientDetailsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private string _clientId;
    private HttpResponse<ClientDTO>? _response;

    public ClientDetailsStepDefinitions(ClientsApi clientsApi)
    {
        _clientsApi = clientsApi;
        _clientId = string.Empty;
    }

    [Given(@"a Client c")]
    public async Task GivenAClient()
    {
        var createClientRequest = new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = "a-client-display-name",
            ClientSecret = string.Empty
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createClientRequest);

        var response = await _clientsApi.CreateClient(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _clientId = response.Content.Result!.ClientId;
    }

    [When(@"a GET request is sent to the /Clients/{c.clientId} endpoint")]
    public async Task WhenAGETRequestIsSentToTheClientsIdEndpoint()
    {
        _response = await _clientsApi.GetClient(_clientId, _requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains Client c")]
    public void ThenTheResponseContainsATier()
    {
        _response!.Content.Result.Should().NotBeNull();
        _response!.Content.Result!.ClientId.Should().NotBeNull();
        _response!.Content.Result!.ClientId.Should().Be(_clientId);
        _response!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}
