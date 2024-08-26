using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
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
    private readonly IdentitiesContext _identitiesContext;
    private readonly RelationshipsContext _relationshipsContext;
    private readonly ResponseContext _responseContext;

    public RelationshipsStepDefinitions(IdentitiesContext identitiesContext, RelationshipsContext relationshipsContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _relationshipsContext = relationshipsContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;

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
        _responseContext.CreateRelationshipTemplateResponse = await ClientPool.FirstForIdentity(identityName)!.RelationshipTemplates.CreateTemplate(CreateRelationshipTemplateRequest);
    }

    [Given("a Relationship ([a-zA-Z0-9]+) in status (Pending|Active|Rejected) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipInStatusBetweenIdentityAndIdentityCreatedByIdentity(string relationshipName, string relationshipStatus, string participant1, string participant2, string identityName)
    {
        var relationshipCreator = ClientPool.FirstForIdentity(identityName)!;
        var relationshipParticipant = ClientPool.FirstForIdentity(identityName == participant1 ? participant2 : participant1)!;

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
        var relationship = await EstablishRelationshipBetween(ClientPool.FirstForIdentity(identity1Name)!, ClientPool.FirstForIdentity(identity2Name)!);
        _relationshipsContext.Relationships[relationshipName] = relationship;
    }

    [Given(@"([a-zA-Z0-9]+) has terminated ([a-zA-Z0-9]+)")]
    public async Task GivenRelationshipIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var terminator = ClientPool.FirstForIdentity(terminatorName)!;

        _responseContext.TerminateRelationshipResponse = await terminator.Relationships.TerminateRelationship(relationship.Id);
        _responseContext.TerminateRelationshipResponse.Should().BeASuccess();
    }

    [Given(@"([a-zA-Z0-9]+) has decomposed ([a-zA-Z0-9]+)")]
    public async Task GivenIdentityHasDecomposedItsRelationshipToIdentity(string decomposerName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var decomposer = ClientPool.FirstForIdentity(decomposerName)!;

        _responseContext.DecomposeRelationshipResponse = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        _responseContext.DecomposeRelationshipResponse.Should().BeASuccess();

        await Task.Delay(500);
    }
    #endregion

    #region When
    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships endpoint with rt.id")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsEndpointWithRelationshipTemplatetId(string identityName)
    {
        var client = ClientPool.FirstForIdentity(identityName)!;
        var relationshipTemplateId = _responseContext.CreateRelationshipTemplateResponse!.Result!.Id;

        _responseContext.WhenResponse = _responseContext.CreateRelationshipResponse = await client.Relationships.CreateRelationship(CreateRelationshipRequest(relationshipTemplateId));
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships/{([a-zA-Z0-9]+).Id}/(Accept|Reject|Revoke) endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipsIdEndpoint(string identityName, string relationshipName, string requestType)
    {
        switch (requestType)
        {
            case "Accept":
                _responseContext.WhenResponse = _responseContext.AcceptRelationshipResponse =
                    await ClientPool.FirstForIdentity(identityName)!.Relationships.AcceptRelationship(_relationshipsContext.Relationships[relationshipName].Id, AcceptRelationshipRequest);
                break;
            case "Reject":
                _responseContext.WhenResponse = _responseContext.RejectRelationshipResponse =
                    await ClientPool.FirstForIdentity(identityName)!.Relationships.RejectRelationship(_relationshipsContext.Relationships[relationshipName].Id, RejectRelationshipRequest);
                break;
            case "Revoke":
                _responseContext.WhenResponse = _responseContext.RevokeRelationshipResponse =
                    await ClientPool.FirstForIdentity(identityName)!.Relationships.RevokeRelationship(_relationshipsContext.Relationships[relationshipName].Id, RevokeRelationshipRequest);
                break;
        }
    }

    [When("a GET request is sent to the /Relationships/CanCreate\\?peer={i.id} endpoint by ([a-zA-Z0-9]+) for ([a-zA-Z0-9]+)")]
    public async Task WhenAGetRequestIsSentToTheCanCreateEndpointByIdentityForIdentity(string identity1Name, string identity2Name)
    {
        _responseContext.WhenResponse = _responseContext.CanEstablishRelationshipResponse = await ClientPool.FirstForIdentity(identity1Name)!.Relationships.CanCreateRelationship(ClientPool.FirstForIdentity(identity2Name)!.IdentityData!.Address);
    }
    #endregion
}

public class RelationshipsContext
{
    public readonly Dictionary<string, Relationship> Relationships = new();
}
