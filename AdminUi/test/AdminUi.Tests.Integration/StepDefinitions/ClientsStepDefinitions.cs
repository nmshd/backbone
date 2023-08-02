using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
[Scope(Feature = "DELETE Clients")]
[Scope(Feature = "PATCH Clients")]
public class ClientsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private string _clientId;
    private string _clientSecret;
    private HttpResponse<List<ClientDTO>>? _clientsResponse;
    private HttpResponse<ClientDTO>? _clientResponse;
    private HttpResponse? _deleteResponse;

    public ClientsStepDefinitions(ClientsApi clientsApi)
    {
        _clientsApi = clientsApi;
        _clientId = string.Empty;
        _clientSecret = string.Empty;
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
        requestConfiguration.SetContent(createClientRequest);

        var response = await _clientsApi.CreateClient(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _clientId = response.Content.Result!.ClientId;
    }

    [Given(@"a non-existent Client c")]
    public void GivenANonExistentClientC()
    {
        _clientId = "some-non-existent-client-id";
    }

    [When(@"a DELETE request is sent to the /Clients endpoint")]
    public async Task WhenADeleteRequestIsSentToTheClientsEndpoint()
    {
        _deleteResponse = await _clientsApi.DeleteClient(_clientId, _requestConfiguration);
        _deleteResponse.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Clients endpoint")]
    public async Task WhenAGetRequestIsSentToTheClientsEndpoint()
    {
        _clientsResponse = await _clientsApi.GetAllClients(_requestConfiguration);
        _clientsResponse.Should().NotBeNull();
        _clientsResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpoint()
    {
        _clientSecret = "new-client-secret";

        var changeClientSecretRequest = new ChangeClientSecretRequest()
        {
            NewSecret = _clientSecret
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(changeClientSecretRequest);

        _clientResponse = await _clientsApi.ChangeClientSecret(_clientId, requestConfiguration);

        _clientResponse.Should().NotBeNull();
        _clientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{clientId}/ChangeSecret endpoint")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointForAnInexistentClient()
    {
        var changeClientSecretRequest = new ChangeClientSecretRequest()
        {
            NewSecret = "new-client-secret"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(changeClientSecretRequest);

        _clientResponse = await _clientsApi.ChangeClientSecret("inexistentClientId", requestConfiguration);

        _clientResponse.Should().NotBeNull();
        _clientResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Clients")]
    public void ThenTheResponseContainsAListOfClients()
    {
        _clientsResponse!.Content.Result.Should().NotBeNull();
        _clientsResponse!.Content.Result.Should().NotBeEmpty();
        _clientsResponse.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains Client c with the new client secret")]
    public void ThenTheResponseContainsAClientWithNewSecret()
    {
        _clientResponse!.AssertHasValue();
        _clientResponse!.AssertStatusCodeIsSuccess();
        _clientResponse!.AssertContentTypeIs("application/json");
        _clientResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_clientsResponse != null)
        {
            var actualStatusCode = (int)_clientsResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_clientResponse != null)
        {
            var actualStatusCode = (int)_clientResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_deleteResponse != null)
        {
            var actualStatusCode = (int)_deleteResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_clientsResponse != null)
        {
            _clientsResponse!.Content.Error.Should().NotBeNull();
            _clientsResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_clientResponse != null)
        {
            _clientResponse!.Content.Error.Should().NotBeNull();
            _clientResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse.Content.Should().NotBeNull();
            _deleteResponse.Content!.Error.Should().NotBeNull();
            _deleteResponse.Content!.Error.Code.Should().Be(errorCode);
        }
    }
}
