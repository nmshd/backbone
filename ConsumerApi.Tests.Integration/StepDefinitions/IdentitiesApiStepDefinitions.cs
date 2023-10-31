using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identities/Self/DeletionProcess")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse? _response;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi) : base(httpConfiguration)
    {
        _identitiesApi = identitiesApi;
    }

    [Given("no active deletion process for the user exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists()
    {
    }

    [When("a POST request is sent to the /Identities/Self/DeletionProcess endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesSelfDeletionProcessEndpoint()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        _response = await _identitiesApi.StartDeletionProcess(requestConfiguration);
    }

    [Then(@"the response status code is (\d\d\d) \(Created\)")]
    public void ThenTheResponseStatusCodeIsCreated(int statusCode)
    {
        ThrowHelpers.ThrowIfNull(_response);
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

}
