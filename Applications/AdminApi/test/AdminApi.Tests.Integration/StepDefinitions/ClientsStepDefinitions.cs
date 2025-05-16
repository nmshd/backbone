using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

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
    private IResponse? _whenResponse;
    private ApiResponse<ListClientsResponse>? _getClientsResponse;
    private ApiResponse<ChangeClientSecretResponse>? _changeClientSecretResponse;
    private ApiResponse<UpdateClientResponse>? _updateClientResponse;
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
        response.Should().BeASuccess();

        var basicTier = response.Result!.SingleOrDefault(t => t.Name == "Basic");
        return basicTier != null ? basicTier.Id : await CreateTier();
    }

    public async Task<string> CreateTier()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + CreateRandomString(12) });
        response.Should().BeASuccess();

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
        response.Should().BeASuccess();

        _clientId = response.Result!.ClientId;
    }

    [Given("a non-existent Client c")]
    public void GivenANonExistentClientC()
    {
        _clientId = "some-non-existent-client-id";
    }

    [When("^a DELETE request is sent to the /Clients endpoint$")]
    public async Task WhenADeleteRequestIsSentToTheClientsEndpoint()
    {
        _whenResponse = _deleteResponse = await _client.Clients.DeleteClient(_clientId);
    }

    [When("^a GET request is sent to the /Clients endpoint$")]
    public async Task WhenAGetRequestIsSentToTheClientsEndpoint()
    {
        _whenResponse = _getClientsResponse = await _client.Clients.GetAllClients();
    }

    [When("^a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint with a new secret$")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithASecret()
    {
        _clientSecret = "new-client-secret";
        _whenResponse = _changeClientSecretResponse = await _client.Clients.ChangeClientSecret(_clientId, new ChangeClientSecretRequest { NewSecret = _clientSecret });
    }

    [When("^a PATCH request is sent to the /Clients/{c.ClientId}/ChangeSecret endpoint without passing a secret$")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointWithoutASecret()
    {
        _whenResponse = _changeClientSecretResponse = await _client.Clients.ChangeClientSecret(_clientId, new ChangeClientSecretRequest { NewSecret = string.Empty });
    }

    [When("^a PATCH request is sent to the /Clients/{clientId}/ChangeSecret endpoint$")]
    public async Task WhenAPatchRequestIsSentToTheClientsChangeSecretEndpointForAnInexistentClient()
    {
        _whenResponse = _changeClientSecretResponse = await _client.Clients.ChangeClientSecret("inexistentClientId", new ChangeClientSecretRequest { NewSecret = "new-client-secret" });
    }

    [When("^a PUT request is sent to the /Clients/{c.ClientId} endpoint$")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpoint()
    {
        _updatedTierId = await CreateTier();
        _updatedMaxIdentities = 150;

        _whenResponse = _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = _updatedTierId,
            MaxIdentities = _updatedMaxIdentities
        });
    }

    [When("^a PUT request is sent to the /Clients/{c.ClientId} endpoint with a null value for maxIdentities$")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithANullMaxIdentities()
    {
        _whenResponse = _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = _tierId,
            MaxIdentities = null
        });
    }

    [When("^a PUT request is sent to the /Clients/{c.ClientId} endpoint with a non-existent tier id$")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointWithAnInexistentDefaultTier()
    {
        _whenResponse = _updateClientResponse = await _client.Clients.UpdateClient(_clientId, new UpdateClientRequest
        {
            DefaultTier = "inexistent-tier-id",
            MaxIdentities = _maxIdentities
        });
    }

    [When("^a PUT request is sent to the /Clients/{c.clientId} endpoint with a non-existing clientId$")]
    public async Task WhenAPatchRequestIsSentToTheClientsEndpointForAnInexistentClient()
    {
        _whenResponse = _updateClientResponse = await _client.Clients.UpdateClient("inexistentClientId", new UpdateClientRequest
        {
            DefaultTier = "new-tier-id",
            MaxIdentities = _maxIdentities
        });
    }

    [Then("the response contains a paginated list of Clients")]
    public async Task ThenTheResponseContainsAListOfClients()
    {
        _getClientsResponse!.Should().BeASuccess();
        _getClientsResponse!.ContentType.Should().StartWith("application/json");
        await _getClientsResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains Client c with the new client secret")]
    public async Task ThenTheResponseContainsAClientWithNewSecret()
    {
        _changeClientSecretResponse!.Should().BeASuccess();
        _changeClientSecretResponse!.ContentType.Should().StartWith("application/json");
        await _changeClientSecretResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains Client c with a random secret generated by the backend")]
    public async Task ThenTheResponseContainsAClientWithRandomGeneratedSecret()
    {
        _changeClientSecretResponse!.Should().BeASuccess();
        _changeClientSecretResponse!.ContentType.Should().StartWith("application/json");
        await _changeClientSecretResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains Client c")]
    public async Task ThenTheResponseContainsAClient()
    {
        _updateClientResponse!.Should().BeASuccess();
        _updateClientResponse!.ContentType.Should().StartWith("application/json");
        await _updateClientResponse!.Should().ComplyWithSchema();
    }

    [Then("the Client in the Backend was successfully updated")]
    public async Task ThenTheClientInTheBackendWasUpdatedAsync()
    {
        var response = await _client.Clients.GetClient(_clientId);

        response.Should().BeASuccess();
        response.Result!.DefaultTier.Should().Be(_updatedTierId);
        response.Result.MaxIdentities.Should().Be(_updatedMaxIdentities);
    }

    [Then("the Client in the Backend has a null value for maxIdentities")]
    public async Task ThenTheClientInTheBackendHasNullIdentitiesLimit()
    {
        var response = await _client.Clients.GetClient(_clientId);

        response.Should().BeASuccess();
        response.Result!.MaxIdentities.Should().BeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.Should().NotBeNull();
        ((int)_whenResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _whenResponse.Should().NotBeNull();
        _whenResponse!.Error.Should().NotBeNull();
        _whenResponse!.Error!.Code.Should().Be(errorCode);
    }
}
