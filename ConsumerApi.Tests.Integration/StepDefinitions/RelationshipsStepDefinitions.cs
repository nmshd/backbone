using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Relationship")]
internal class RelationshipsStepDefinitions
{
    private Client _client1 = null!;
    private Client _client2 = null!;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<CreateRelationshipTemplateResponse>? _relationshipTemplateResponse;
    private ApiResponse<RelationshipMetadata>? _createRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? _acceptRelationshipChangeResponse;
    private ApiResponse<RelationshipMetadata>? _rejectRelationshipChangeResponse;
    private ApiResponse<RelationshipMetadata>? _revokeRelationshipChangeResponse;
    private string _relationshipId = string.Empty;
    private string _relationshipChangeId = string.Empty;

    public RelationshipsStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

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
        _relationshipChangeId = createRelationshipResponse.Result!.Changes.First().Id;
    }

    [Given("a pending Relationship between i1 and i2 created by i2")]
    public async Task GivenAPendingRelationshipBetweenI1AndI2CreatedByI1()
    {
        var relationshipTemplateResponse = await CreateRelationshipTemplate(_client1);
        var createRelationshipResponse = await CreateRelationship(_client2, relationshipTemplateResponse.Result!.Id);

        _relationshipId = createRelationshipResponse.Result!.Id;
        _relationshipChangeId = createRelationshipResponse!.Result!.Changes.First().Id;
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

    [When("a POST request is sent to the /Relationships endpoint by i1 with rt.id")]
    public async Task WhenAPostRequestIsSentToTheRelationshipsEndpointByI1With()
    {
        _createRelationshipResponse = await CreateRelationship(_client1, _relationshipTemplateResponse!.Result!.Id);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Accept endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheAcceptRelationshipChangeEndpointByI1()
    {
        var completeRelationshipChangeRequest = new CompleteRelationshipChangeRequest
        {
            Content = "AAA".GetBytes()
        };
        _acceptRelationshipChangeResponse = await _client1.Relationships.AcceptChange(_relationshipId, _relationshipChangeId, completeRelationshipChangeRequest);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Reject endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheRejectRelationshipChangeEndpointByI1()
    {
        var completeRelationshipChangeRequest = new CompleteRelationshipChangeRequest
        {
            Content = "AAA".GetBytes()
        };
        _rejectRelationshipChangeResponse = await _client1.Relationships.RejectChange(_relationshipId, _relationshipChangeId, completeRelationshipChangeRequest);
    }

    [When("a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Revoke endpoint by i1")]
    public async Task WhenAPostRequestIsSentToTheRevokeRelationshipChangeEndpointByI2()
    {
        var completeRelationshipChangeRequest = new CompleteRelationshipChangeRequest
        {
            Content = "AAA".GetBytes()
        };
        _revokeRelationshipChangeResponse = await _client1.Relationships.RevokeChange(_relationshipId, _relationshipChangeId, completeRelationshipChangeRequest);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_createRelationshipResponse != null)
            ((int)_createRelationshipResponse!.Status).Should().Be(expectedStatusCode);

        if (_acceptRelationshipChangeResponse != null)
            ((int)_acceptRelationshipChangeResponse!.Status).Should().Be(expectedStatusCode);

        if (_rejectRelationshipChangeResponse != null)
            ((int)_rejectRelationshipChangeResponse!.Status).Should().Be(expectedStatusCode);

        if (_revokeRelationshipChangeResponse != null)
            ((int)_revokeRelationshipChangeResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_createRelationshipResponse != null)
        {
            _createRelationshipResponse!.Error.Should().NotBeNull();
            _createRelationshipResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_acceptRelationshipChangeResponse != null)
        {
            _acceptRelationshipChangeResponse!.Error.Should().NotBeNull();
            _acceptRelationshipChangeResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_rejectRelationshipChangeResponse != null)
        {
            _rejectRelationshipChangeResponse!.Error.Should().NotBeNull();
            _rejectRelationshipChangeResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_revokeRelationshipChangeResponse != null)
        {
            _revokeRelationshipChangeResponse!.Error.Should().NotBeNull();
            _revokeRelationshipChangeResponse.Error!.Code.Should().Be(errorCode);
        }
    }

    [Then("the response contains a Relationship")]
    public void ThenTheResponseContainsARelationship()
    {
        _createRelationshipResponse!.Should().BeASuccess();
        _createRelationshipResponse!.Should().ComplyWithSchema();
    }

    [Then("the response contains an AcceptRelationshipChangeResponse")]
    public void ThenTheResponseContainsAnAcceptRelationshipChangeResponse()
    {
        _acceptRelationshipChangeResponse!.Should().BeASuccess();
        _acceptRelationshipChangeResponse!.Should().ComplyWithSchema();
    }

    [Then("the response contains an RejectRelationshipChangeResponse")]
    public void ThenTheResponseContainsAnRejectRelationshipChangeResponse()
    {
        _rejectRelationshipChangeResponse!.Should().BeASuccess();
        _rejectRelationshipChangeResponse!.Should().ComplyWithSchema();
    }

    [Then("the response contains an RevokeRelationshipChangeResponse")]
    public void ThenTheResponseContainsAnRevokeRelationshipChangeResponse()
    {
        _revokeRelationshipChangeResponse!.Should().BeASuccess();
        _revokeRelationshipChangeResponse!.Should().ComplyWithSchema();
    }

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
