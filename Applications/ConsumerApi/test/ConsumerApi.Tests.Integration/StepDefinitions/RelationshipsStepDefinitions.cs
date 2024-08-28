using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Tooling.Extensions;
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

    private static CreateRelationshipTemplateRequest CreateRelationshipTemplateRequest => new() { Content = "AAA".GetBytes() };
    private static CreateRelationshipRequest CreateRelationshipRequest(string relationshipTemplateId) => new() { RelationshipTemplateId = relationshipTemplateId, Content = "AAA".GetBytes() };
    private static AcceptRelationshipRequest AcceptRelationshipRequest => new() { CreationResponseContent = "AAA".GetBytes() };
    private static RejectRelationshipRequest RejectRelationshipRequest => new() { CreationResponseContent = "AAA".GetBytes() };
    private static RevokeRelationshipRequest RevokeRelationshipRequest => new() { CreationResponseContent = "AAA".GetBytes() };

    #endregion

    #region Given

    [Given("a Relationship Template rt created by ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipTemplateCreatedByIdentity(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.CreateRelationshipTemplateResponse = await client.RelationshipTemplates.CreateTemplate(CreateRelationshipTemplateRequest);
    }

    [Given("a Relationship ([a-zA-Z0-9]+) in status (Pending|Active|Rejected) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipInStatusBetweenIdentityAndIdentityCreatedByIdentity(string relationshipName, string relationshipStatus, string participant1, string participant2,
        string identityName)
    {
        var relationshipCreator = _clientPool.FirstForIdentityName(identityName);
        var relationshipParticipant = _clientPool.FirstForIdentityName(identityName == participant1 ? participant2 : participant1);

        _relationshipsContext.Relationships[relationshipName] = relationshipStatus switch
        {
            "Pending" => await CreatePendingRelationshipBetween(relationshipParticipant, relationshipCreator),
            "Active" => await EstablishRelationshipBetween(relationshipParticipant, relationshipCreator),
            "Rejected" => await CreateRejectedRelationshipBetween(relationshipParticipant, relationshipCreator),
            _ => _relationshipsContext.Relationships[relationshipName]
        };
    }

    [Given(@"a Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipBetweenIdentityAndIdentity(string relationshipName, string identity1Name, string identity2Name)
    {
        var relationship = await EstablishRelationshipBetween(_clientPool.FirstForIdentityName(identity1Name), _clientPool.FirstForIdentityName(identity2Name));
        _relationshipsContext.Relationships[relationshipName] = relationship;
    }

    [Given(@"([a-zA-Z0-9]+) has terminated ([a-zA-Z0-9]+)")]
    public async Task GivenRelationshipIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var terminator = _clientPool.FirstForIdentityName(terminatorName);

        _responseContext.TerminateRelationshipResponse = await terminator.Relationships.TerminateRelationship(relationship.Id);
        _responseContext.TerminateRelationshipResponse.Should().BeASuccess();
    }

    [Given(@"([a-zA-Z0-9]+) has decomposed ([a-zA-Z0-9]+)")]
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

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships endpoint with rt.id")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsEndpointWithRelationshipTemplatetId(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var relationshipTemplateId = _responseContext.CreateRelationshipTemplateResponse!.Result!.Id;

        _responseContext.WhenResponse = _responseContext.CreateRelationshipResponse = await client.Relationships.CreateRelationship(CreateRelationshipRequest(relationshipTemplateId));
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships/{([a-zA-Z0-9]+).Id}/(Accept|Reject|Revoke) endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdEndpoint(string identityName, string relationshipName, string requestType)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = requestType switch
        {
            "Accept" => _responseContext.AcceptRelationshipResponse =
                await client.Relationships.AcceptRelationship(_relationshipsContext.Relationships[relationshipName].Id, AcceptRelationshipRequest),
            "Reject" => _responseContext.RejectRelationshipResponse =
                await client.Relationships.RejectRelationship(_relationshipsContext.Relationships[relationshipName].Id, RejectRelationshipRequest),
            "Revoke" => _responseContext.RevokeRelationshipResponse =
                await client.Relationships.RevokeRelationship(_relationshipsContext.Relationships[relationshipName].Id, RevokeRelationshipRequest),
            _ => _responseContext.WhenResponse
        };
    }

    [When("a GET request is sent to the /Relationships/CanCreate\\?peer={i.id} endpoint by ([a-zA-Z0-9]+) for ([a-zA-Z0-9]+)")]
    public async Task WhenAGetRequestIsSentToTheCanCreateEndpointByIdentityForIdentity(string identity1Name, string identity2Name)
    {
        var client = _clientPool.FirstForIdentityName(identity1Name);
        _responseContext.WhenResponse = _responseContext.CanEstablishRelationshipResponse =
            await client.Relationships.CanCreateRelationship(_clientPool.FirstForIdentityName(identity2Name).IdentityData!.Address);
    }

    #endregion
}

public class RelationshipsContext
{
    public readonly Dictionary<string, Relationship> Relationships = new();
}
