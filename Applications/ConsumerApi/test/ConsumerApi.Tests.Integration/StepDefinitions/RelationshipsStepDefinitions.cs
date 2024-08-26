using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Relationship")]
[Scope(Feature = "GET Relationship/CanCreate")]
internal class RelationshipsStepDefinitions
{
    private Client _client1 = null!;
    private Client _client2 = null!;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<CreateRelationshipTemplateResponse>? _relationshipTemplateResponse;
    private ApiResponse<RelationshipMetadata>? _createRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? _acceptRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? _rejectRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? _revokeRelationshipResponse;
    private ApiResponse<CanEstablishRelationshipResponse>? _canEstablishResponse;
    private string _relationshipId = string.Empty;

    public RelationshipsStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    #region Given

    [Given("Identities i1 and i2")]
    public async Task GivenIdentitiesI1AndI2()
    {
        _client1 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
        _client2 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
    }

    [Given("a Relationship Template rt created by i2")]
    public async Task GivenARelationshipTemplateRtCreatedByI2()
    {
        _relationshipTemplateResponse = await CreateRelationshipTemplate(_client2);
    }

    [Given("a pending Relationship between i1 and i2 created by i1")]
    public async Task GivenAPendingRelationshipBetweenI1AndI2CreatedByI2()
    {
        var relationshipTemplateResponse = await CreateRelationshipTemplate(_client2);
        var createRelationshipResponse = await CreateRelationship(_client1, relationshipTemplateResponse.Result!.Id);

        _relationshipId = createRelationshipResponse.Result!.Id;
    }

    [Given("a pending Relationship between i1 and i2 created by i2")]
    public async Task GivenAPendingRelationshipBetweenI1AndI2CreatedByI1()
    {
        var relationshipTemplateResponse = await CreateRelationshipTemplate(_client1);
        var createRelationshipResponse = await CreateRelationship(_client2, relationshipTemplateResponse.Result!.Id);

        _relationshipId = createRelationshipResponse.Result!.Id;
    }

    [Given("an active Relationship between i1 and i2 created by i1")]
    public async Task GivenAnActiveRelationshipBetweenI1AndI2()
    {
        _relationshipId = (await Utils.EstablishRelationshipBetween(_client1, _client2)).Id;
    }

    [Given("a rejected Relationship between i1 and i2 created by i1")]
    public async Task GivenARejectedRelationshipBetweenI1AndI2()
    {
        _relationshipId = (await Utils.CreateRejectedRelationshipBetween(_client1, _client2)).Id;
    }

    [Given("i2 is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityI2IsToBeDeleted()
    {
        var startDeletionProcessResponse = await _client2.Identities.StartDeletionProcess();
        startDeletionProcessResponse.Should().BeASuccess();
    }

    [Given("i1 is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityI1IsToBeDeleted()
    {
        var startDeletionProcessResponse = await _client1.Identities.StartDeletionProcess();
        startDeletionProcessResponse.Should().BeASuccess();
    }

    #endregion

    #region When

    [When("a POST request is sent to the /Relationships endpoint by i1 with rt.id")]
    public async Task WhenAPostRequestIsSentToTheRelationshipsEndpointByI1With()
    {
        _createRelationshipResponse = await CreateRelationship(_client1, _relationshipTemplateResponse!.Result!.Id);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Accept endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheAcceptRelationshipEndpointByI1()
    {
        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _acceptRelationshipResponse = await _client1.Relationships.AcceptRelationship(_relationshipId, acceptRelationshipRequest);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Reject endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheRejectRelationshipEndpointByI1()
    {
        var rejectRelationshipRequest = new RejectRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _rejectRelationshipResponse = await _client1.Relationships.RejectRelationship(_relationshipId, rejectRelationshipRequest);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Revoke endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheRevokeRelationshipEndpointByI2()
    {
        var revokeRelationshipRequest = new RevokeRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _revokeRelationshipResponse = await _client1.Relationships.RevokeRelationship(_relationshipId, revokeRelationshipRequest);
    }

    [When("a GET request is sent to the /Relationships/CanCreate\\?peer={i.id} endpoint by i1 for i2")]
    public async Task WhenAGetRequestIsSentToTheCanCreateEndpointByI1()
    {
        _canEstablishResponse = await _client1.Relationships.CanCreateRelationship(_client2.IdentityData!.Address);
    }

    #endregion

    #region Then

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_createRelationshipResponse != null)
            ((int)_createRelationshipResponse!.Status).Should().Be(expectedStatusCode);

        if (_acceptRelationshipResponse != null)
            ((int)_acceptRelationshipResponse!.Status).Should().Be(expectedStatusCode);

        if (_rejectRelationshipResponse != null)
            ((int)_rejectRelationshipResponse!.Status).Should().Be(expectedStatusCode);

        if (_revokeRelationshipResponse != null)
            ((int)_revokeRelationshipResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_createRelationshipResponse != null)
        {
            _createRelationshipResponse!.Error.Should().NotBeNull();
            _createRelationshipResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_acceptRelationshipResponse != null)
        {
            _acceptRelationshipResponse!.Error.Should().NotBeNull();
            _acceptRelationshipResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_rejectRelationshipResponse != null)
        {
            _rejectRelationshipResponse!.Error.Should().NotBeNull();
            _rejectRelationshipResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_revokeRelationshipResponse != null)
        {
            _revokeRelationshipResponse!.Error.Should().NotBeNull();
            _revokeRelationshipResponse.Error!.Code.Should().Be(errorCode);
        }
    }

    [Then("the response contains a RelationshipResponse")]
    public async Task ThenTheResponseContainsARelationship()
    {
        if (_createRelationshipResponse != null)
        {
            _createRelationshipResponse!.Should().BeASuccess();
            await _createRelationshipResponse!.Should().ComplyWithSchema();
        }

        if (_acceptRelationshipResponse != null)
        {
            _acceptRelationshipResponse!.Should().BeASuccess();
            await _acceptRelationshipResponse!.Should().ComplyWithSchema();
        }

        if (_rejectRelationshipResponse != null)
        {
            _rejectRelationshipResponse!.Should().BeASuccess();
            await _rejectRelationshipResponse!.Should().ComplyWithSchema();
        }

        if (_revokeRelationshipResponse != null)
        {
            _revokeRelationshipResponse!.Should().BeASuccess();
            await _revokeRelationshipResponse!.Should().ComplyWithSchema();
        }
    }

    [Then("a relationship can be established")]
    public void ThenARelationshipCanBeEstablished() => PerformCanEstablishRelationshipCheck(true);

    [Then("a relationship can not be established")]
    public void ThenARelationshipCanNotBeEstablished() => PerformCanEstablishRelationshipCheck(false);

    private void PerformCanEstablishRelationshipCheck(bool value)
    {
        if (_canEstablishResponse != null)
        {
            _canEstablishResponse!.Should().BeASuccess();
            _canEstablishResponse!.Result!.CanCreate.Should().Be(value);
        }
    }

    #endregion

    private async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateRelationshipTemplate(Client client)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = "AAA".GetBytes()
        };

        return await client.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
    }

    private async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(Client client, string relationshipTemplateId)
    {
        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateId,
            Content = "AAA".GetBytes()
        };

        return await client.Relationships.CreateRelationship(createRelationshipRequest);
    }
}
