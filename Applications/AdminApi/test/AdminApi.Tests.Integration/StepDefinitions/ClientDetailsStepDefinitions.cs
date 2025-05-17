using Backbone.AdminApi.Sdk.Endpoints.Clients.Types;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Client Details")]
internal class ClientDetailsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<ClientInfo>? _response;
    private string _clientId;
    private string _tierId;
    private readonly int _maxIdentities;

    public ClientDetailsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _clientId = string.Empty;
        _tierId = string.Empty;
        _maxIdentities = 1;
    }

    [Given("a Tier t")]
    public async Task GivenATierT()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest
        {
            Name = "TestTier_" + CreateRandomString(12)
        });
        response.Should().BeASuccess();

        _tierId = response.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given("a Client c with Tier t")]
    public async Task GivenAClientWithTierT()
    {
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

    [When("^a GET request is sent to the /Clients/{c.clientId} endpoint$")]
    public async Task WhenAGETRequestIsSentToTheClientsIdEndpoint()
    {
        _response = await _client.Clients.GetClient(_clientId);
    }

    [Then("the response contains Client c")]
    public async Task ThenTheResponseContainsAClient()
    {
        _response!.Result!.Should().NotBeNull();
        _response!.Result!.ClientId.Should().Be(_clientId);
        _response!.Result!.DefaultTier.Should().Be(_tierId);
        _response!.Result!.MaxIdentities.Should().Be(_maxIdentities);
        _response!.ContentType.Should().StartWith("application/json");
        await _response!.Should().ComplyWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_response!.Status).Should().Be(expectedStatusCode);
    }
}
