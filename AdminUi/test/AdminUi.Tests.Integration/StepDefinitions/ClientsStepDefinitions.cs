using Backbone.AdminUi.Tests.Integration.API;
using Backbone.AdminUi.Tests.Integration.Extensions;
using Backbone.AdminUi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
[Scope(Feature = "DELETE Clients")]
[Scope(Feature = "PATCH Clients")]
internal class ClientsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private readonly TiersApi _tiersApi;
    private string _clientId;
    private string _clientSecret;
    private string _tierId;
    private string _tier1Id;
    private string _tier2Id;
    private int _maxIdentities;
    private int _maxIdentitiesNew;
    private HttpResponse<List<ClientOverviewDTO>>? _getClientsResponse;
    private readonly HttpResponse<ClientDTO>? _getClientResponse;
    private readonly HttpResponse<CreateClientResponse>? _createClientResponse;
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
        _tier1Id = string.Empty;
        _tier2Id = string.Empty;
        _maxIdentities = 0;
        _maxIdentitiesNew = 0;
    }

    [Given(@"a non-existent Client c")]
    public void GivenANonExistentClientC()
    {
        _clientId = "some-non-existent-client-id";
    }

    [Given(@"a Tier t")]
    public async Task GivenATierT()
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
        _tierId = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given(@"a MaxIdentities mi")]
    public void GivenAMaxIdentitiesMi()
    {
        _maxIdentities = 1;
    }

    [Given(@"a MaxIdentities miNew")]
    public void GivenAMaxIdentitiesMiNew()
    {
        _maxIdentitiesNew = 2;
    }

    [Given(@"a Tier t1")]
    public async Task GivenATierT1()
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
        _tier1Id = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given(@"a Tier t2")]
    public async Task GivenATierT2()
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
        _tier2Id = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given(@"a Client c with Tier t")]
    public async Task GivenAClientCWithTierT()
    {
        var createClientRequest = new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = string.Empty,
            ClientSecret = string.Empty,
            DefaultTier = _tierId
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createClientRequest);

        var response = await _clientsApi.CreateClient(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _clientId = response.Content.Result!.ClientId;
    }

    [Given(@"a Client c with Tier t and MaxIdentities maxIdentities")]
    public async Task GivenAClientCWithTierTAndMaxIdentitiesMaxIdentities()
    {
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

    [Given(@"a Client c with Tier t1")]
    public async Task GivenAClientCWithTierT1()
    {
        var createClientRequest = new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = string.Empty,
            ClientSecret = string.Empty,
            DefaultTier = _tier1Id
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createClientRequest);

        var response = await _clientsApi.CreateClient(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _clientId = response.Content.Result!.ClientId;
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

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId} endpoint with the defaultTier t2.Id")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithTier2Id()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = _tier2Id
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId} endpoint with the maxIdentities maxIdentitiesNew")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithMaxIdentitiesNew()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = _tierId,
            MaxIdentities = _maxIdentitiesNew
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId} endpoint with a null value for maxIdentities")]
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

    [When(@"a PATCH request is sent to the /Clients/{c.ClientId} endpoint with a non-existent tier id")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithAnInexistentDefaultTier()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = "inexistent-tier-id"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(updateClientRequest);

        _updateClientResponse = await _clientsApi.UpdateClient(_clientId, requestConfiguration);

        _updateClientResponse.Should().NotBeNull();
        _updateClientResponse.Content.Should().NotBeNull();
    }

    [When(@"a PATCH request is sent to the /Clients/{c.clientId} endpoint with a non-existing clientId")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointForAnInexistentClient()
    {
        var updateClientRequest = new UpdateClientRequest()
        {
            DefaultTier = "new-tier-id"
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

    [Then(@"the Client in the Backend has the new defaultTier")]
    public async Task ThenTheClientInTheBackendHasNewDefaultTier()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _clientsApi.GetClient(_clientId, requestConfiguration);

        response.AssertHasValue();
        response.AssertStatusCodeIsSuccess();
        response.AssertContentTypeIs("application/json");
        response.AssertContentCompliesWithSchema();
        response.Content.Result.DefaultTier.Should().Be(_tier2Id);
    }

    [Then(@"the Client in the Backend has the new maxIdentities")]
    public async Task ThenTheClientInTheBackendHasMaxIdentitiesNew()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _clientsApi.GetClient(_clientId, requestConfiguration);

        response.AssertHasValue();
        response.AssertStatusCodeIsSuccess();
        response.AssertContentTypeIs("application/json");
        response.AssertContentCompliesWithSchema();
        response.Content.Result.MaxIdentities.Should().Be(_maxIdentitiesNew);
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
        response.Content.Result.MaxIdentities.Should().BeNull();
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

        if (_updateClientResponse != null)
        {
            _updateClientResponse!.Content.Error.Should().NotBeNull();
            _updateClientResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}
