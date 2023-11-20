using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Models;
using CSharpFunctionalExtensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identities/Self/DeletionProcess")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse<StartDeletionProcessResponse>? _response;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi) : base(httpConfiguration)
    {
        _identitiesApi = identitiesApi;
    }

    [Given("no active deletion process for the identity exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists()
    {
    }

    [Given("an active deletion process for the identity exists")]
    public async Task GivenAnActiveDeletionProcessForTheUserExists()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        await _identitiesApi.StartDeletionProcess(requestConfiguration);
    }

    [When("a POST request is sent to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesSelfDeletionProcessEndpoint()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        _response = await _identitiesApi.StartDeletionProcess(requestConfiguration);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIsCreated(int statusCode)
    {
        ThrowHelpers.ThrowIfNull(_response);
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Content.Should().NotBeNull();
        _response.Content!.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }

    [Then("the response contains a Deletion Process")]
    public void ThenTheResponseContainsADeletionProcess()
    {
        _response!.Content.Should().NotBeNull();
        _response!.Content.Result.Should().NotBeNull();
        _response!.AssertContentCompliesWithSchema();
    }
}
