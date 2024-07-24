using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
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
//[Scope(Feature = "POST Relationship")]
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
    private string _relationshipId = string.Empty;

    private readonly IdentitiesContext _identitiesContext;
    private readonly RelationshipsContext _relationshipsContext;
    private readonly ResponseContext _responseContext;

    public RelationshipsStepDefinitions(IdentitiesContext identitiesContext, RelationshipsContext relationshipsContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _identitiesContext = identitiesContext;
        _relationshipsContext = relationshipsContext;
        _responseContext = responseContext;
    }

    //[Given("Identities i1 and i2")]
    //public async Task GivenIdentitiesI1AndI2()
    //{
    //    _client1 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
    //    _client2 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
    //}

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

    //[Given("i2 is in status \"ToBeDeleted\"")]
    //public async Task GivenIdentityI2IsToBeDeleted()
    //{
    //    var startDeletionProcessResponse = await _client2.Identities.StartDeletionProcess();
    //    startDeletionProcessResponse.Should().BeASuccess();
    //}

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

    //[Then(@"the response status code is (\d\d\d) \(.+\)")]
    //public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    //{
    //    if (_createRelationshipResponse != null)
    //        ((int)_createRelationshipResponse!.Status).Should().Be(expectedStatusCode);

    //    if (_acceptRelationshipResponse != null)
    //        ((int)_acceptRelationshipResponse!.Status).Should().Be(expectedStatusCode);

    //    if (_rejectRelationshipResponse != null)
    //        ((int)_rejectRelationshipResponse!.Status).Should().Be(expectedStatusCode);

    //    if (_revokeRelationshipResponse != null)
    //        ((int)_revokeRelationshipResponse!.Status).Should().Be(expectedStatusCode);
    //}

    //[Then(@"the response content contains an error with the error code ""([^""]*)""")]
    //public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    //{
    //    if (_createRelationshipResponse != null)
    //    {
    //        _createRelationshipResponse!.Error.Should().NotBeNull();
    //        _createRelationshipResponse.Error!.Code.Should().Be(errorCode);
    //    }

    //    if (_acceptRelationshipResponse != null)
    //    {
    //        _acceptRelationshipResponse!.Error.Should().NotBeNull();
    //        _acceptRelationshipResponse.Error!.Code.Should().Be(errorCode);
    //    }

    //    if (_rejectRelationshipResponse != null)
    //    {
    //        _rejectRelationshipResponse!.Error.Should().NotBeNull();
    //        _rejectRelationshipResponse.Error!.Code.Should().Be(errorCode);
    //    }

    //    if (_revokeRelationshipResponse != null)
    //    {
    //        _revokeRelationshipResponse!.Error.Should().NotBeNull();
    //        _revokeRelationshipResponse.Error!.Code.Should().Be(errorCode);
    //    }
    //}

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

    // refactoring

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    [Given(@"a Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipRBetweenIAndI(string relationshipName, string identity1Name, string identity2Name)
    {
        var relationship = await Utils.EstablishRelationshipBetween(Identity(identity1Name), Identity(identity2Name));
        _relationshipsContext.Relationships[relationshipName] = relationship;
    }

    [Given(@"([a-zA-Z0-9]+) has terminated ([a-zA-Z0-9]+)")]
    public async Task GivenRIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var terminator = Identity(terminatorName);

        _responseContext.TerminateRelationshipResponse = await terminator.Relationships.TerminateRelationship(relationship.Id);
        _responseContext.TerminateRelationshipResponse.Should().BeASuccess();
    }

    [Given(@"([a-zA-Z0-9]+) has decomposed ([a-zA-Z0-9]+)")]
    public async Task GivenIHasDecomposedItsRelationshipToI(string decomposerName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var decomposer = Identity(decomposerName);

        _responseContext.DecomposeRelationshipResponse = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        _responseContext.DecomposeRelationshipResponse.Should().BeASuccess();

        await Task.Delay(500);
    }
}

public class RelationshipsContext
{
    public readonly Dictionary<string, Relationship> Relationships = new();
}
