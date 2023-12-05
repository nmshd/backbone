using Backbone.AdminUi.Tests.Integration.API;
using Backbone.AdminUi.Tests.Integration.Extensions;
using Backbone.AdminUi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
[Scope(Feature = "DELETE Clients")]
[Scope(Feature = "PATCH Clients")]
[Scope(Feature = "PUT Clients")]
internal class ClientsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private readonly TiersApi _tiersApi;
    private string _clientId;
    private string _clientSecret;
    private string _tierId;
    private string _updatedTierId;
    private int? _maxIdentities;
    private int? _updatedMaxIdentities;
    private HttpResponse<List<ClientOverviewDTO>>? _getClientsResponse;
    private HttpResponse<ChangeClientSecretResponse>? _changeClientSecretResponse;
    private HttpResponse<UpdateClientResponse>? _updateClientResponse;
    private HttpResponse? _deleteResponse;

    public ClientsStepDefinitions(ClientsApi clientsApi, TiersApi tiersApi)
    {
        _clientsApi = clientsApi;
        _tiersApi = tiersApi;
        _clientId = string.Empty;
        _clientSecret = string.Empty;
        _tierId = string.Empty;
        _updatedTierId = string.Empty;
        _maxIdentities = 0;
        _updatedMaxIdentities = 0;
    }

    public async Task<string> GetTier()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _tiersApi.GetTiers(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(200);

        var basicTier = response.Content.Result!.SingleOrDefault(t => t.Name == "Basic");

        return basicTier != null ? basicTier.Id : await CreateTier();
    }

    public async Task<string> CreateTier()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = "TestTier_" + TestDataGenerator.GenerateString(12)
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        var response = await _tiersApi.CreateTier(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);

        return response.Content.Result!.Id;
    }

    [Given(@"a Client c")]
    public async Task GivenAClientC()
    {
        _tierId = await GetTier();
        _maxIdentities = 100;

        var createClientRequest = new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = string.Empty,
            ClientSecret = string.Empty,
            DefaultTier = _tierId,
            MaxIdentities = _maxIdentities
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

        var changeClientSecretRequest = new ChangeClientSecretRequest
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

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint without passing a secret")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithoutASecret()
    {
        var changeClientSecretRequest = new ChangeClientSecretRequest
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
        var changeClientSecretRequest = new ChangeClientSecretRequest
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

    [When(@"a PUT request is sent to the /Clients/{c.ClientId} endpoint")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpoint()
    {
        _updatedTierId = await CreateTier();
        _updatedMaxIdentities = 150;

        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = _updatedTierId,
            MaxIdentities = _updatedMaxIdentities
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PUT request is sent to the /Clients/{c.ClientId} endpoint with a null value for maxIdentities")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithANullMaxIdentities()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = _tierId,
            MaxIdentities = null
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PUT request is sent to the /Clients/{c.ClientId} endpoint with a non-existent tier id")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithAnInexistentDefaultTier()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = "inexistent-tier-id",
            MaxIdentities = _maxIdentities
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PUT request is sent to the /Clients/{c.clientId} endpoint with a non-existing clientId")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointForAnInexistentClient()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = "new-tier-id",
            MaxIdentities = _maxIdentities
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient("inexistentClientId", requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
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

    [Then(@"the response contains Client c with a random secret generated by the backend")]
    public void ThenTheResponseContainsAClientWithRandomGeneratedSecret()
    {
        _changeClientSecretResponse!.AssertHasValue();
        _changeClientSecretResponse!.AssertStatusCodeIsSuccess();
        _changeClientSecretResponse!.AssertContentTypeIs("application/json");
        _changeClientSecretResponse!.AssertContentCompliesWithSchema();
        _changeClientSecretResponse!.Content.Result!.ClientSecret.Should().NotBeNullOrEmpty();
    }

    [Then(@"the response contains Client c")]
    public void ThenTheResponseContainsAClient()
    {
        _updateClientResponse!.AssertHasValue();
        _updateClientResponse!.AssertStatusCodeIsSuccess();
        _updateClientResponse!.AssertContentTypeIs("application/json");
        _updateClientResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the Client in the Backend was successfully updated")]
    public async Task ThenTheClientInTheBackendWasUpdatedAsync()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _clientsApi.GetClient(_clientId, requestConfiguration);

        response.AssertHasValue();
        response.AssertStatusCodeIsSuccess();
        response.AssertContentTypeIs("application/json");
        response.AssertContentCompliesWithSchema();
        response.Content.Result!.DefaultTier.Should().Be(_updatedTierId);
        response.Content.Result!.MaxIdentities.Should().Be(_updatedMaxIdentities);
    }

    [Then(@"the Client in the Backend has a null value for maxIdentities")]
    public async Task ThenTheClientInTheBackendHasNullIdentitiesLimit()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _clientsApi.GetClient(_clientId, requestConfiguration);

        response.AssertHasValue();
        response.AssertStatusCodeIsSuccess();
        response.AssertContentTypeIs("application/json");
        response.AssertContentCompliesWithSchema();
        response.Content.Result!.MaxIdentities.Should().BeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_getClientsResponse != null)
        {
            var actualStatusCode = (int)_getClientsResponse.StatusCode;
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

        if (_updateClientResponse != null)
        {
            var actualStatusCode = (int)_updateClientResponse.StatusCode;
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

        if (_updateClientResponse != null)
        {
            _updateClientResponse!.Content.Error.Should().NotBeNull();
            _updateClientResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}
