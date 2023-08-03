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
    private HttpResponse<List<ClientDTO>>? _getClientsResponse;
    private readonly HttpResponse<ClientDTO>? _getClientResponse;
    private readonly HttpResponse<CreateClientResponse>? _createClientResponse;
    private HttpResponse<ChangeClientSecretResponse>? _changeClientSecretResponse;
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
        _getClientsResponse = await _clientsApi.GetAllClients(_requestConfiguration);
        _getClientsResponse.Should().NotBeNull();
        _getClientsResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint with a new secret")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithASecret()
    {
        _clientSecret = "new-client-secret";

        var changeClientSecretRequest = new ChangeClientSecretRequest()
        {
            NewSecret = _clientSecret
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(changeClientSecretRequest);

        _changeClientSecretResponse = await _clientsApi.ChangeClientSecret(_clientId, requestConfiguration);

        _changeClientSecretResponse.Should().NotBeNull();
        _changeClientSecretResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint with an empty new secret")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithAnEmptySecret()
    {
        var changeClientSecretRequest = new ChangeClientSecretRequest()
        {
            NewSecret = string.Empty
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(changeClientSecretRequest);

        _changeClientSecretResponse = await _clientsApi.ChangeClientSecret(_clientId, requestConfiguration);

        _changeClientSecretResponse.Should().NotBeNull();
        _changeClientSecretResponse.Content.Should().NotBeNull();
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

        _changeClientSecretResponse = await _clientsApi.ChangeClientSecret("inexistentClientId", requestConfiguration);

        _changeClientSecretResponse.Should().NotBeNull();
        _changeClientSecretResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a paginated list of Clients")]
    public void ThenTheResponseContainsAListOfClients()
    {
        _getClientsResponse!.Content.Result.Should().NotBeNullOrEmpty();
        _getClientsResponse.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains Client c with the new client secret")]
    public void ThenTheResponseContainsAClientWithNewSecret()
    {
        _changeClientSecretResponse!.AssertHasValue();
        _changeClientSecretResponse!.AssertStatusCodeIsSuccess();
        _changeClientSecretResponse!.AssertContentTypeIs("application/json");
        _changeClientSecretResponse!.AssertContentCompliesWithSchema();
        _changeClientSecretResponse!.Content.Result!.ClientSecret.Should().Be(_clientSecret);
    }

    [Then(@"the response contains Client c with a newly generated client secret")]
    public void ThenTheResponseContainsAClientWithNewlyGeneratedSecret()
    {
        _changeClientSecretResponse!.AssertHasValue();
        _changeClientSecretResponse!.AssertStatusCodeIsSuccess();
        _changeClientSecretResponse!.AssertContentTypeIs("application/json");
        _changeClientSecretResponse!.AssertContentCompliesWithSchema();
        _changeClientSecretResponse!.Content.Result!.ClientSecret.Should().NotBeNullOrEmpty();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_getClientsResponse != null)
        {
            var actualStatusCode = (int)_getClientsResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_getClientResponse != null)
        {
            var actualStatusCode = (int)_getClientResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_createClientResponse != null)
        {
            var actualStatusCode = (int)_createClientResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_changeClientSecretResponse != null)
        {
            var actualStatusCode = (int)_changeClientSecretResponse.StatusCode;
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
        if (_getClientsResponse != null)
        {
            _getClientsResponse!.Content.Error.Should().NotBeNull();
            _getClientsResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_getClientResponse != null)
        {
            _getClientResponse!.Content.Error.Should().NotBeNull();
            _getClientResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_createClientResponse != null)
        {
            _createClientResponse!.Content.Error.Should().NotBeNull();
            _createClientResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_changeClientSecretResponse != null)
        {
            _changeClientSecretResponse!.Content.Error.Should().NotBeNull();
            _changeClientSecretResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse.Content.Should().NotBeNull();
            _deleteResponse.Content!.Error.Should().NotBeNull();
            _deleteResponse.Content!.Error.Code.Should().Be(errorCode);
        }
    }
}
