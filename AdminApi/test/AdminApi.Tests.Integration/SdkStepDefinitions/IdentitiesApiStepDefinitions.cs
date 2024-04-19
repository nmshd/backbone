﻿using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
[Scope(Feature = "POST Identities/{id}/DeletionProcess")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    //private ODataResponse<List<IdentityOverviewDTO>>? _identityOverviewsResponse;
    private ApiResponse<GetIdentityResponse>? _identityResponse;
    private ApiResponse<CreateIdentityResponse>? _createIdentityResponse;
    private ApiResponse<StartDeletionProcessAsSupportResponse>? _identityDeletionProcessResponse;
    private string _existingIdentity;

    public IdentitiesApiStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _existingIdentity = string.Empty;
    }

    [Given("an Identity i")]
    public async Task GivenAnIdentityI()
    {
        var accountController = new AccountController(_client);
        _createIdentityResponse = await accountController.CreateIdentity(_options.ClientId, _options.ClientSecret) ?? throw new InvalidOperationException();
        _createIdentityResponse.IsSuccess.Should().BeTrue();

        _existingIdentity = _createIdentityResponse.Result!.Result!.Address;
    }

    [Given("an active deletion process for Identity i exists")]
    public async Task GivenAnActiveDeletionProcessForIdentityAExists()
    {
        await _client.Identities.StartDeletionProcess(_createIdentityResponse!.Result!.Result!.Address);
    }

    [When("a POST request is sent to the /Identities/{i.id}/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesIdDeletionProcessesEndpoint()
    {
        _identityDeletionProcessResponse = await _client.Identities.StartDeletionProcess(_createIdentityResponse!.Result!.Result!.Address);
    }

    [When("a GET request is sent to the /Identities endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesOverviewEndpoint()
    {
        //_identityOverviewsResponse = await _client.Identities.GetIdentityOverviews();
    }

    [When("a GET request is sent to the /Identities/{i.address} endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpoint()
    {
        _identityResponse = await _client.Identities.GetIdentity(_existingIdentity);
    }

    [When("a GET request is sent to the /Identities/{address} endpoint with an inexistent address")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpointForAnInexistentIdentity()
    {
        _identityResponse = await _client.Identities.GetIdentity("inexistentIdentityAddress");
    }

    [Then("the response contains a list of Identities")]
    public void ThenTheResponseContainsAListOfIdentities()
    {
        //_identityOverviewsResponse!.Result!.Result.Should().NotBeNull();
        //_identityOverviewsResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the response contains a Deletion Process")]
    public void ThenTheResponseContainsADeletionProcess()
    {
        _identityDeletionProcessResponse!.Result!.Result.Should().NotBeNull();
        _identityDeletionProcessResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the response contains Identity i")]
    public void ThenTheResponseContainsAnIdentity()
    {
        _identityResponse!.Result!.Result.Should().NotBeNull();
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_identityResponse != null)
            ((int)_identityResponse!.Status).Should().Be(expectedStatusCode);

        //if (_identityOverviewsResponse != null)
        //    ((int)_postResponse!.Status).Should().Be(expectedStatusCode);

        if (_identityDeletionProcessResponse != null)
            ((int)_identityDeletionProcessResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_identityResponse != null)
        {
            _identityResponse!.Result!.Error.Should().NotBeNull();
            _identityResponse.Result!.Error!.Code.Should().Be(errorCode);
        }

        if (_identityDeletionProcessResponse != null)
        {
            _identityDeletionProcessResponse!.Result!.Error.Should().NotBeNull();
            _identityDeletionProcessResponse.Result!.Error!.Code.Should().Be(errorCode);
        }
    }
}
