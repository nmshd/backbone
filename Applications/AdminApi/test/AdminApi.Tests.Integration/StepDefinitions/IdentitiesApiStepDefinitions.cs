using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
[Scope(Feature = "POST Identities/{id}/DeletionProcess")]
[Scope(Feature = "GET Identities/{identityAddress}")]
[Scope(Feature = "GET Identities/{identityAddress}/DeletionProcesses/AuditLogs")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<ListIdentitiesResponse>? _identityOverviewsResponse;
    private ApiResponse<GetIdentityResponse>? _identityResponse;
    private ApiResponse<CreateIdentityResponse>? _createIdentityResponse;
    private ApiResponse<StartDeletionProcessAsSupportResponse>? _identityDeletionProcessResponse;
    private ApiResponse<ListIdentityDeletionProcessAuditLogsResponse>? _identityDeletionProcessAuditLogsResponse;
    private IResponse? _whenResponse;
    private string _existingIdentity;

    public IdentitiesApiStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _existingIdentity = string.Empty;
    }

    [Given("an Identity i")]
    public async Task GivenAnIdentityI()
    {
        _createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);
        _createIdentityResponse.ShouldBeASuccess();

        _existingIdentity = _createIdentityResponse.Result!.Address;
    }

    [Given("an active deletion process for Identity i exists")]
    public async Task GivenAnActiveDeletionProcessForIdentityAExists()
    {
        await _client.Identities.StartDeletionProcess(_createIdentityResponse!.Result!.Address);
    }

    [When("^a POST request is sent to the /Identities/{i.id}/DeletionProcesses endpoint$")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesIdDeletionProcessesEndpoint()
    {
        _whenResponse = _identityDeletionProcessResponse = await _client.Identities.StartDeletionProcess(_createIdentityResponse!.Result!.Address);
    }

    [When("^a GET request is sent to the /Identities endpoint$")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesOverviewEndpoint()
    {
        _whenResponse = _identityOverviewsResponse = await _client.Identities.ListIdentities();
    }

    [When("^a GET request is sent to the /Identities/{i.address}/DeletionProcesses/AuditLogs endpoint$")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesDeletionProcessesAuditLogsEndpoint()
    {
        _whenResponse = _identityDeletionProcessAuditLogsResponse = await _client.Identities.ListIdentityDeletionProcessAuditLogs(_existingIdentity);
    }

    [When("^a GET request is sent to the /Identities/{i.address} endpoint$")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpoint()
    {
        _whenResponse = _identityResponse = await _client.Identities.GetIdentity(_existingIdentity);
    }

    [When("^a GET request is sent to the /Identities/{address} endpoint with an inexistent address$")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpointForAnInexistentIdentity()
    {
        _whenResponse = _identityResponse = await _client.Identities.GetIdentity(CreateRandomIdentityAddress());
    }

    [Then("the response contains a list of Identities")]
    public async Task ThenTheResponseContainsAListOfIdentities()
    {
        _identityOverviewsResponse!.Result!.ShouldNotBeNull();
        _identityOverviewsResponse!.ContentType.ShouldStartWith("application/json");
        await _identityOverviewsResponse.ShouldComplyWithSchema();
    }

    [Then("the response contains a list of Identity Deletion Process Audit Logs")]
    public async Task ThenTheResponseContainsAListOfIdentityDeletionProcessAuditLogs()
    {
        _identityDeletionProcessAuditLogsResponse!.Result!.ShouldNotBeNull();
        _identityDeletionProcessAuditLogsResponse!.ContentType.ShouldStartWith("application/json");
        await _identityDeletionProcessAuditLogsResponse.ShouldComplyWithSchema();
    }

    [Then("the response contains a Deletion Process")]
    public async Task ThenTheResponseContainsADeletionProcess()
    {
        _identityDeletionProcessResponse!.Result!.ShouldNotBeNull();
        _identityDeletionProcessResponse!.ContentType.ShouldStartWith("application/json");
        await _identityDeletionProcessResponse.ShouldComplyWithSchema();
    }

    [Then("the response contains Identity i")]
    public async Task ThenTheResponseContainsAnIdentity()
    {
        _identityResponse!.Result!.ShouldNotBeNull();
        _identityResponse!.ContentType.ShouldStartWith("application/json");
        await _identityResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();
        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _whenResponse.ShouldNotBeNull();
        _whenResponse!.Error.ShouldNotBeNull();
        _whenResponse.Error!.Code.ShouldBe(errorCode);
    }
}
