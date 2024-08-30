using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;
using AcceptRelationshipRequest = Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests.AcceptRelationshipRequest;
using RejectRelationshipRequest = Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests.RejectRelationshipRequest;
using RevokeRelationshipRequest = Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests.RevokeRelationshipRequest;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipsStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly RelationshipsContext _relationshipsContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

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

        _relationshipsContext.Relationships[relationshipName] = await CreatePendingRelationshipBetween(peer, creator);
    }

    [Given($"a rejected Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenARejectedRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await CreateRejectedRelationshipBetween(participant2, participant1);
    }

    [Given($"an active Relationship {RegexFor.SINGLE_THING} between {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public async Task GivenAnActiveRelationshipBetween(string relationshipName, string participant1Address, string participant2Address)
    {
        var participant1 = _clientPool.FirstForIdentityName(participant1Address);
        var participant2 = _clientPool.FirstForIdentityName(participant2Address);

        _relationshipsContext.Relationships[relationshipName] = await EstablishRelationshipBetween(participant2, participant1);
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

        _responseContext.DecomposeRelationshipResponse = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        _responseContext.DecomposeRelationshipResponse.Should().BeASuccess();

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

    // TODO: remove i.id from the step definition

    [When($"a GET request is sent to the /Relationships/CanCreate\\?peer={{i.id}} endpoint by {RegexFor.SINGLE_THING} for {RegexFor.SINGLE_THING}")]
    public async Task WhenAGetRequestIsSentToTheCanCreateEndpointByIdentityForIdentity(string identity1Name, string identity2Name)
    {
        var client = _clientPool.FirstForIdentityName(identity1Name);
        _responseContext.WhenResponse = _responseContext.CanEstablishRelationshipResponse =
            await client.Relationships.CanCreateRelationship(_clientPool.FirstForIdentityName(identity2Name).IdentityData!.Address);
    }

    #endregion

    #region Then

    [Then("a relationship can be established")]
    public void ThenARelationshipCanBeEstablished()
    {
        if (_responseContext.CanEstablishRelationshipResponse != null)
            _responseContext.CanEstablishRelationshipResponse.Result!.CanCreate.Should().BeTrue();
    }

    [Then("a relationship can not be established")]
    public void ThenARelationshipCanNotBeEstablished()
    {
        if (_responseContext.CanEstablishRelationshipResponse != null)
            _responseContext.CanEstablishRelationshipResponse.Result!.CanCreate.Should().BeFalse();
    }

    #endregion
}
