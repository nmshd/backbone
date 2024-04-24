using Backbone.AdminApi.Sdk.Endpoints.Clients.Types;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.UnitTestTools.Data;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "GET Clients")]
[Scope(Feature = "DELETE Clients")]
[Scope(Feature = "PATCH Clients")]
[Scope(Feature = "PUT Clients")]
internal class ClientsStepDefinitions : BaseStepDefinitions
{
    private string _clientId;
    private string _clientSecret;
    private string _tierId;
    private string _updatedTierId;
    private int? _maxIdentities;
    private int? _updatedMaxIdentities;
    private ApiResponse<ListClientsResponse>? _getClientsResponse;
    private ApiResponse<ClientInfo>? _changeClientSecretResponse;
    private ApiResponse<ClientInfo>? _updateClientResponse;
    private ApiResponse<EmptyResponse>? _deleteResponse;

    public ClientsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _clientId = string.Empty;
        _clientSecret = string.Empty;
        _tierId = string.Empty;
        _updatedTierId = string.Empty;
        _maxIdentities = 0;
        _updatedMaxIdentities = 0;
    }

    public async Task<string> GetTier()
    {
        var response = await _client.Tiers.ListTiers();
        response.IsSuccess.Should().BeTrue();

        var basicTier = response.Result!.SingleOrDefault(t => t.Name == "Basic");
        return basicTier != null ? basicTier.Id : await CreateTier();
    }

    public async Task<string> CreateTier()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + TestDataGenerator.GenerateString(12) });
        response.IsSuccess.Should().BeTrue();

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);

        return response.Result!.Id;
    }

    [Given("a Client c")]
    public async Task GivenAClientC()
    {
        _tierId = await GetTier();
        _maxIdentities = 100;

        var response = await _client.Clients.CreateClient(new CreateClientRequest
        {
            ClientId = string.Empty,
            DisplayName = string.Empty,
            ClientSecret = string.Empty,
            DefaultTier = _tierId,
            MaxIdentities = _maxIdentities
        });
        response.IsSuccess.Should().BeTrue();

        _clientId = response.Result!.ClientId;
    }

    [Given("a non-existent Client c")]
    public void GivenANonExistentClientC()
    {
        _clientId = "some-non-existent-client-id";
    }

    [When("a DELETE request is sent to the /Clients endpoint")]
    public async Task WhenADeleteRequestIsSentToTheClientsEndpoint()
    {
        _deleteResponse = await _client.Clients.DeleteClient(_clientId);
    }

    [When("a GET request is sent to the /Clients endpoint")]
    public async Task WhenAGetRequestIsSentToTheClientsEndpoint()
    {
        _getClientsResponse = await _client.Clients.GetAllClients();
    }

    [When("a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint with a new secret")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithASecret()
    {
        _clientSecret = "new-client-secret";
        _changeClientSecretResponse = await _client.Clients.ChangeClientSecret(_clientId, new ChangeClientSecretRequest { NewSecret = _clientSecret });
    }

    [When("a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint without passing a secret")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithoutASecret()
    {
        _changeClientSecretResponse = await _client.Clients.ChangeClientSecret(_clientId, new ChangeClientSecretRequest { NewSecret = string.Empty });
    }

    [When("a PATCH request is sent to the /Clients/{clientId}/ChangeSecret endpoint")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointForAnInexistentClient()
    {
        _changeClientSecretResponse = await _client.Clients.ChangeClientSecret("inexistentClientId", new ChangeClientSecretRequest { NewSecret = "new-client-secret" });
    }

    [When("a PUT request is sent to the /Clients/{c.ClientId} endpoint")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpoint()
    {
        _updatedTierId = await CreateTier();
        _updatedMaxIdentities = 150;

        _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = _updatedTierId,
            MaxIdentities = _updatedMaxIdentities
        });
    }

    [When("a PUT request is sent to the /Clients/{c.ClientId} endpoint with a null value for maxIdentities")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithANullMaxIdentities()
    {
        _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = _tierId,
            MaxIdentities = null
        });
    }

    [When("a PUT request is sent to the /Clients/{c.ClientId} endpoint with a non-existent tier id")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithAnInexistentDefaultTier()
    {
        _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = "inexistent-tier-id",
            MaxIdentities = _maxIdentities
        });
    }

    [When("a PUT request is sent to the /Clients/{c.clientId} endpoint with a non-existing clientId")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointForAnInexistentClient()
    {
        _updateClientResponse = await _client.Clients.UpdateClient("inexistentClientId", new UpdateClientRequest
        {
            DefaultTier = "new-tier-id",
            MaxIdentities = _maxIdentities
        });
    }

    [Then("the response contains a paginated list of Clients")]
    public void ThenTheResponseContainsAListOfClients()
    {
        _getClientsResponse!.IsSuccess.Should().BeTrue();
        _getClientsResponse.AssertContentCompliesWithSchema();
    }

    [Then("the response contains Client c with the new client secret")]
    public void ThenTheResponseContainsAClientWithNewSecret()
    {
        _changeClientSecretResponse!.IsSuccess.Should().BeTrue();
        _changeClientSecretResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the response contains Client c with a random secret generated by the backend")]
    public void ThenTheResponseContainsAClientWithRandomGeneratedSecret()
    {
        _changeClientSecretResponse!.IsSuccess.Should().BeTrue();
        _changeClientSecretResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the response contains Client c")]
    public void ThenTheResponseContainsAClient()
    {
        _updateClientResponse!.IsSuccess.Should().BeTrue();
        _updateClientResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the Client in the Backend was successfully updated")]
    public async Task ThenTheClientInTheBackendWasUpdatedAsync()
    {
        var response = await _client.Clients.GetClient(_clientId);

        response.IsSuccess.Should().BeTrue();
        response.Result!.DefaultTier.Should().Be(_updatedTierId);
        response.Result.MaxIdentities.Should().Be(_updatedMaxIdentities);
    }

    [Then("the Client in the Backend has a null value for maxIdentities")]
    public async Task ThenTheClientInTheBackendHasNullIdentitiesLimit()
    {
        var response = await _client.Clients.GetClient(_clientId);

        response.IsSuccess.Should().BeTrue();
        response.Result!.MaxIdentities.Should().BeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_getClientsResponse != null)
            ((int)_getClientsResponse!.Status).Should().Be(expectedStatusCode);

        if (_changeClientSecretResponse != null)
            ((int)_changeClientSecretResponse!.Status).Should().Be(expectedStatusCode);

        if (_deleteResponse != null)
            ((int)_deleteResponse!.Status).Should().Be(expectedStatusCode);

        if (_updateClientResponse != null)
            ((int)_updateClientResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_getClientsResponse != null)
        {
            _getClientsResponse!.Error.Should().NotBeNull();
            _getClientsResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_changeClientSecretResponse != null)
        {
            _changeClientSecretResponse!.Error.Should().NotBeNull();
            _changeClientSecretResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse.Error.Should().NotBeNull();
            _deleteResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_updateClientResponse != null)
        {
            _updateClientResponse.Error.Should().NotBeNull();
            _updateClientResponse.Error!.Code.Should().Be(errorCode);
        }
    }
}
