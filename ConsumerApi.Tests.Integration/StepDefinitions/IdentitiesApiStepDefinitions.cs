﻿using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Models;
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
    private HttpResponse? _response;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi) : base(httpConfiguration)
    {
        _identitiesApi = identitiesApi;
    }

    [Given("no active deletion process for the user exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists()
    {
        //var requestConfiguration = new RequestConfiguration();
        //requestConfiguration.SupplementWith(_requestConfiguration);
        //requestConfiguration.Authenticate = true;
        //requestConfiguration.AuthenticationParameters.Username = "USRa";
        //requestConfiguration.AuthenticationParameters.Password = "a";
        //requestConfiguration.ContentType = "application/json";
        //requestConfiguration.Content = JsonConvert.SerializeObject(new CreateIdentityRequest
        //{
        //    ClientId = "test",
        //    ClientSecret = "test",
        //    IdentityPublicKey = "eyJwdWIiOiJJZDVWb3RUUkFTczJWb1RGQjl5dUV4ZUNIQkM4Rkt4N0pOenpVUEhUbGFJIiwiYWxnIjozLCJAdHlwZSI6IkNyeXB0b1NpZ25hdHVyZVB1YmxpY0tleSJ9",
        //    DevicePassword = "test",
        //    IdentityVersion = 1,
        //    SignedChallenge = new CreateIdentityRequestSignedChallenge
        //    {
        //        Challenge = "{\"id\": \"CHLOzq3LUZDz4xUA3yDo\",\"expiresAt\": \"2023-10-09T10:22:52.486Z\"",
        //        Signature = "eyJzaWciOiJjdWZ6T1laNTdJRDZ4NXFiN0pyajN2TG9weGlpREY5S0xZNDdNbVJkODFQNVN4cV9jOXd0QXpWbGttekdLNlFFVXBfQnVjNjlzNTN5aV9WSHBtaEtCZyIsImFsZyI6MiwiQHR5cGUiOiJDcnlwdG9TaWduYXR1cmUifQ"
        //    }
        //});

        //var response = await _identitiesApi.CreateIdentity(requestConfiguration);
        //response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Given(@"an active deletion process for the user exists")]
    public async Task GivenAnActiveDeletionProcessForTheUserExists()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        await _identitiesApi.StartDeletionProcess(requestConfiguration);
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
}
