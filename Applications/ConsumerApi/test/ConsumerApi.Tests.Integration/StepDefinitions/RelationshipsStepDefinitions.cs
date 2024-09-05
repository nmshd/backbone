using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipsStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly RelationshipsContext _relationshipsContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<CanEstablishRelationshipResponse>? _canEstablishRelationshipResponse;

    public RelationshipsStepDefinitions(RelationshipsContext relationshipsContext, ResponseContext responseContext, ClientPool clientPool)
    {
        _relationshipsContext = relationshipsContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"a Relationship Template {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenARelationshipTemplateCreatedByIdentity(string templateName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _relationshipsContext.CreateRelationshipTemplateResponses[templateName] =
            (await client.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES })).Result!;
    }

    [Given($"a pending Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenAPendingRelationshipBetween(string relationshipName, string participant1Name, string participant2Name, string creatorName)
    {
        var creator = _clientPool.FirstForIdentityName(creatorName);
        var peer = _clientPool.FirstForIdentityName(creatorName == participant1Name ? participant2Name : participant1Name);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreatePendingRelationshipBetween(peer, creator);
    }

    [Given($"a rejected Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenARejectedRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreateRejectedRelationshipBetween(participant2, participant1);
    }

    [Given($"an active Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenAnActiveRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.EstablishRelationshipBetween(participant2, participant1);
    }

    [Given($"a terminated Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenATerminatedRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreateTerminatedRelationshipBetween(participant1, participant2);
    }

    [Given($"a terminated Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING} with reactivation request by second participant")]
    public async Task GivenATerminatedRelationshipWithReactivationRequest(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreateTerminatedRelationshipWithReactivationRequestBetween(participant2, participant1);
    }

    [Given($"{RegexFor.SINGLE_THING} has terminated {RegexFor.SINGLE_THING}")]
    public async Task GivenRelationshipIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var terminator = _clientPool.FirstForIdentityName(terminatorName);

        await terminator.Relationships.TerminateRelationship(relationship.Id);
    }

    [Given($"{RegexFor.SINGLE_THING} has decomposed {RegexFor.SINGLE_THING}")]
    public async Task GivenIdentityHasDecomposedItsRelationshipToIdentity(string decomposerName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var decomposer = _clientPool.FirstForIdentityName(decomposerName);

        var response = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        response.Should().BeASuccess();

        await Task.Delay(500);
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Relationships endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsEndpointWithRelationshipTemplateId(string identityName, string templateName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationshipTemplateId = _relationshipsContext.CreateRelationshipTemplateResponses[templateName].Id;

        _responseContext.WhenResponse =
            await client.Relationships.CreateRelationship(new CreateRelationshipRequest { RelationshipTemplateId = relationshipTemplateId, Content = TestData.SOME_BYTES });
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/(Accept|Reject|Revoke) endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdEndpoint(string identityName, string relationshipName, string requestType)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = requestType switch
        {
            "Accept" => await client.Relationships.AcceptRelationship(_relationshipsContext.Relationships[relationshipName].Id,
                new AcceptRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES }),
            "Reject" => await client.Relationships.RejectRelationship(_relationshipsContext.Relationships[relationshipName].Id,
                new RejectRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES }),
            "Revoke" => await client.Relationships.RevokeRelationship(_relationshipsContext.Relationships[relationshipName].Id,
                new RevokeRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES }),
            _ => throw new NotSupportedException($"Unsupported request type: {requestType}")
        };
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Terminate endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheRelationshipsIdEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.Relationships.TerminateRelationship(_relationshipsContext.Relationships[relationshipName].Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Reactivate endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheRelationshipsIdReactivateEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.Relationships.RelationshipReactivationRequest(_relationshipsContext.Relationships[relationshipName].Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Reactivate/(Accept|Reject|Revoke) endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheRelationshipsIdReactivateAcceptEndpoint(string identityName, string relationshipName, string requestType)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = requestType switch
        {
            "Accept" => await client.Relationships.AcceptReactivationOfRelationship(_relationshipsContext.Relationships[relationshipName].Id),
            "Reject" => await client.Relationships.RejectReactivationOfRelationship(_relationshipsContext.Relationships[relationshipName].Id),
            "Revoke" => await client.Relationships.RevokeRelationshipReactivation(_relationshipsContext.Relationships[relationshipName].Id),
            _ => throw new NotSupportedException($"Unsupported request type: {requestType}")
        };
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Decompose endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheRelationshipsIdDecomposeEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.Relationships.DecomposeRelationship(_relationshipsContext.Relationships[relationshipName].Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Relationships/CanCreate\\?peer={{id}} endpoint with id={RegexFor.SINGLE_THING}.id")]
    public async Task WhenAGetRequestIsSentToTheCanCreateEndpointByIdentityForIdentity(string activeIdentityName, string peerName)
    {
        var client = _clientPool.FirstForIdentityName(activeIdentityName);
        _responseContext.WhenResponse = _canEstablishRelationshipResponse =
            await client.Relationships.CanCreateRelationship(_clientPool.FirstForIdentityName(peerName).IdentityData!.Address);
    }

    #endregion

    #region Then

    [Then("a Relationship can be established")]
    public void ThenARelationshipCanBeEstablished()
    {
        if (_canEstablishRelationshipResponse != null)
            _canEstablishRelationshipResponse.Result!.CanCreate.Should().BeTrue();
    }

    [Then("a Relationship can not be established")]
    public void ThenARelationshipCanNotBeEstablished()
    {
        if (_canEstablishRelationshipResponse != null)
            _canEstablishRelationshipResponse.Result!.CanCreate.Should().BeFalse();
    }

    #endregion
}
