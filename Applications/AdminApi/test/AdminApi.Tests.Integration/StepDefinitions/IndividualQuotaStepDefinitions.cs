using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST IndividualQuota")]
[Scope(Feature = "DELETE IndividualQuota")]
internal class IndividualQuotaStepDefinitions : BaseStepDefinitions
{
    private string _identityAddress;
    private string _quotaId;
    private ApiResponse<IndividualQuota>? _createQuotaResponse;
    private ApiResponse<EmptyResponse>? _deleteResponse;
    private IResponse? _whenResponse;
    
    public IndividualQuotaStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _identityAddress = string.Empty;
        _quotaId = string.Empty;
    }

    [Given("an Identity i")]
    public async Task GivenAnIdentityI()
    {
        await CreateIdentity();
    }

    [Given("an Identity i with an IndividualQuota q")]
    public async Task GivenAnIdentityIWithAnIndividualQuotaQ()
    {
        await CreateIdentity();

        var response = await _client.Identities.CreateIndividualQuota(_identityAddress, new CreateQuotaForIdentityRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
        response.Should().BeASuccess();

        _quotaId = response.Result!.Id;
    }

    [When("a DELETE request is sent to the /Identities/{i.address}/Quotas/{q.id} endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteIndividualQuotaEndpoint()
    {
        _whenResponse = _deleteResponse = await _client.Identities.DeleteIndividualQuota(_identityAddress, _quotaId);
    }

    [When("a DELETE request is sent to the /Identities/{i.address}/Quotas/inexistentQuotaId endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteIndividualQuotaEndpointWithAnInexistentQuotaId()
    {
        _whenResponse = _deleteResponse = await _client.Identities.DeleteIndividualQuota(_identityAddress, "QUOInexistentIdxxxxx");
    }

    [When("a DELETE request is sent to the /Identities/{nonExistentAddress}/Quotas/{q.id} endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteIndividualQuotaEndpointWithANonExistentIdentityAddress()
    {
        _whenResponse = _deleteResponse = await _client.Identities.DeleteIndividualQuota("someNonExistentIdentityAddress", _quotaId);
    }

    [When("a POST request is sent to the /Identity/{i.id}/Quotas endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheCreateIndividualQuotaEndpoint()
    {
        _whenResponse = _createQuotaResponse = await _client.Identities.CreateIndividualQuota(_identityAddress, new CreateQuotaForIdentityRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("a POST request is sent to the /Identity/{address}/Quotas endpoint with an inexistent identity address")]
    public async Task WhenAPOSTRequestIsSentToTheCreateIndividualQuotaEndpointWithAnInexistentIdentityAddress()
    {
        _whenResponse = _createQuotaResponse = await _client.Identities.CreateIndividualQuota("some-inexistent-identity-address", new CreateQuotaForIdentityRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("a POST request is sent to the /Identity/{i.id}/Quotas endpoint with an invalid metric key")]
    public async Task WhenAPOSTRequestIsSentToTheCreateIndividualQuotaEndpointWithAnInvalidMetricKey()
    {
        _whenResponse = _createQuotaResponse = await _client.Identities.CreateIndividualQuota(_identityAddress, new CreateQuotaForIdentityRequest
        {
            MetricKey = "someInvalidMetricKey",
            Max = 2,
            Period = "Week"
        });
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.Should().NotBeNull();

        ((int)_whenResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains an IndividualQuota")]
    public async Task ThenTheResponseContainsAnIndividualQuota()
    {
        _createQuotaResponse!.Result!.Should().NotBeNull();
        _createQuotaResponse!.ContentType.Should().StartWith("application/json");
        await _createQuotaResponse.Should().ComplyWithSchema();
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_createQuotaResponse != null)
        {
            _createQuotaResponse!.Error.Should().NotBeNull();
            _createQuotaResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse.Error.Should().NotBeNull();
            _deleteResponse.Error!.Code.Should().Be(errorCode);
        }
    }

    private async Task CreateIdentity()
    {
        var createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);
        createIdentityResponse.Should().BeASuccess();

        _identityAddress = createIdentityResponse.Result!.Address;

        // allow the event queue to trigger the creation of this Identity on the Quotas module
        Thread.Sleep(2000);
    }
}
