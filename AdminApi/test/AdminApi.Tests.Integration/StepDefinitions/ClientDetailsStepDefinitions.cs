using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;
internal class ClientDetailsStepDefinitions : BaseStepDefinitions
{
    private readonly ClientsApi _clientsApi;
    private readonly TiersApi _tiersApi;
    private string _clientId;
    private string _tierId;
    private readonly int _maxIdentities;
    private HttpResponse<ClientDTO>? _response;

    public ClientDetailsStepDefinitions(ClientsApi clientsApi, TiersApi tiersApi)
    {
        _clientsApi = clientsApi;
        _tiersApi = tiersApi;
        _clientId = string.Empty;
        _tierId = string.Empty;
        _maxIdentities = 1;
    }

    [Given("a Tier t")]
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

    [Given("a Client c with Tier t")]
    public async Task GivenAClientWithTierT()
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

    [When("a GET request is sent to the /Clients/{c.clientId} endpoint")]
    public async Task WhenAGETRequestIsSentToTheClientsIdEndpoint()
    {
        _response = await _clientsApi.GetClient(_clientId, _requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then("the response contains Client c")]
    public void ThenTheResponseContainsAClient()
    {
        _response!.Content.Result.Should().NotBeNull();
        _response!.Content.Result!.ClientId.Should().NotBeNull();
        _response!.Content.Result!.ClientId.Should().Be(_clientId);
        _response!.Content.Result!.DefaultTier.Should().NotBeNull();
        _response!.Content.Result!.DefaultTier.Should().Be(_tierId);
        _response!.Content.Result!.MaxIdentities.Should().NotBeNull();
        _response!.Content.Result!.MaxIdentities.Should().Be(_maxIdentities);
        _response!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}
