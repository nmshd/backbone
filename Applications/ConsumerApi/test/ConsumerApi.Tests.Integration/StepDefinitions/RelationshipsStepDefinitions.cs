using System.Net;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipsStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly RelationshipsContext _relationshipsContext;
    private readonly RelationshipTemplatesContext _relationshipTemplatesContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<CanEstablishRelationshipResponse>? _canEstablishRelationshipResponse;

    public RelationshipsStepDefinitions(RelationshipsContext relationshipsContext, RelationshipTemplatesContext relationshipTemplatesContext, ResponseContext responseContext, ClientPool clientPool)
    {
        _relationshipsContext = relationshipsContext;
        _relationshipTemplatesContext = relationshipTemplatesContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"a pending Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenAPendingRelationshipBetween(string relationshipName, string requestorName, string templatorName, string creatorName)
    {
        var creator = _clientPool.FirstForIdentityName(creatorName);
        var peer = _clientPool.FirstForIdentityName(creatorName == requestorName ? templatorName : requestorName);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreatePendingRelationshipBetween(peer, creator);
    }

    [Given($"a pending Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenAPendingRelationshipBetween(string relationshipName, string requestorName, string templatorName)
    {
        var templator = _clientPool.FirstForIdentityName(requestorName);
        var requestor = _clientPool.FirstForIdentityName(templatorName);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreatePendingRelationshipBetween(templator, requestor);
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

    [Given($"an active Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING} with template {RegexFor.SINGLE_THING}")]
    public async Task GivenAnActiveRelationshipBetween(string relationshipName, string participant1Address, string participant2Address, string templateName)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);
        var template = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[templateName];

        _relationshipsContext.Relationships[relationshipName] = await Utils.EstablishRelationshipBetween(participant1, participant2, template.Id);
    }

    [Given($"a terminated Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenATerminatedRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreateTerminatedRelationshipBetween(participant1, participant2);
    }

    [Given($"a terminated Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING} with reactivation requested by i2")]
    public async Task GivenATerminatedRelationshipWithReactivationRequest(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await Utils.CreateTerminatedRelationshipWithReactivationRequestBetween(participant1, participant2);
    }

    [Given($"{RegexFor.SINGLE_THING} was accepted")]
    public async Task GivenRWasAccepted(string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];

        var clientTo = _clientPool.FirstForIdentityAddress(relationship.To);

        await clientTo.Relationships.AcceptRelationship(relationship.Id, new AcceptRelationshipRequest());
    }

    [Given($"{RegexFor.SINGLE_THING} was fully reactivated")]
    public async Task GivenRWasFullyReactivated(string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];

        var clientFrom = _clientPool.FirstForIdentityAddress(relationship.From);
        await clientFrom.Relationships.ReactivateRelationship(relationship.Id);

        var clientTo = _clientPool.FirstForIdentityAddress(relationship.To);
        await clientTo.Relationships.AcceptReactivationOfRelationship(relationship.Id);
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
        response.ShouldBeASuccess();
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Relationships endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsEndpointWithRelationshipTemplateId(string identityName, string templateName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationshipTemplateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[templateName].Id;

        _responseContext.WhenResponse =
            await client.Relationships.CreateRelationship(new CreateRelationshipRequest { RelationshipTemplateId = relationshipTemplateId, Content = TestData.SOME_BYTES });
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Accept endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdAcceptEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationship = _relationshipsContext.Relationships[relationshipName];
        _responseContext.WhenResponse = await client.Relationships.AcceptRelationship(relationship.Id, new AcceptRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES });
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Reject endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdRejectEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationship = _relationshipsContext.Relationships[relationshipName];
        _responseContext.WhenResponse = await client.Relationships.RejectRelationship(relationship.Id, new RejectRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES });
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Relationships/{{{RegexFor.SINGLE_THING}.Id}}/Revoke endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdRevokeEndpoint(string identityName, string relationshipName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationship = _relationshipsContext.Relationships[relationshipName];
        _responseContext.WhenResponse = await client.Relationships.RevokeRelationship(relationship.Id, new RevokeRelationshipRequest { CreationResponseContent = TestData.SOME_BYTES });
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

        _responseContext.WhenResponse = await client.Relationships.ReactivateRelationship(_relationshipsContext.Relationships[relationshipName].Id);
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

    [When($"^{RegexFor.SINGLE_THING} sends a GET request to the /Relationships/CanCreate\\?peer={{id}} endpoint with id={RegexFor.SINGLE_THING}.id$")]
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
        _canEstablishRelationshipResponse!.Result!.CanCreate.ShouldBeTrue();
    }

    [Then("a Relationship can not be established")]
    public void ThenARelationshipCanNotBeEstablished()
    {
        _canEstablishRelationshipResponse!.Result!.CanCreate.ShouldBeFalse();
    }

    [Then(@"the relationship creation check code is ""(.+)""")]
    public void ThenTheCodeIs(string code)
    {
        _canEstablishRelationshipResponse!.Result!.Code.ShouldBe(code);
    }

    [Then(@"the response does not contain a relationship creation check code")]
    public void ThenThereIsNoCode()
    {
        _canEstablishRelationshipResponse!.Result!.Code.ShouldBeNull();
    }

    [Then($"the Relationship {RegexFor.SINGLE_THING} still exists")]
    public async Task ThenTheRelationshipStillExists(string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var client = _clientPool.FirstForIdentityAddress(relationship.From);

        var getRelationshipResponse = await client.Relationships.GetRelationship(relationship.Id);
        getRelationshipResponse.Status.ShouldBe(HttpStatusCode.OK);

        _relationshipsContext.Relationships[relationshipName] = getRelationshipResponse.Result!;
    }

    [Then($"the Relationship {RegexFor.SINGLE_THING} does not have a relationship template")]
    public void ThenTheRelationshipDoesNotHaveARelationshipTemplate(string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        relationship.RelationshipTemplateId.ShouldBeNull();
    }

    #endregion
}
