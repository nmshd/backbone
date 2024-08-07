using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Tooling.Extensions;

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
    #endregion

    #region Given
    [Given("a Relationship Template rt created by ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipTemplateRtCreatedByI(string identityName)
    {
        _responseContext.CreateRelationshipTemplateResponse = await CreateRelationshipTemplate(ClientPool.FirstForIdentity(identityName)!);
    }

    [Given("a pending Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenAPendingRelationshipBetweenIAndICreatedByI(string relationshipName, string participant1, string participant2, string identityName)
    {
        var relationshipCreator = ClientPool.FirstForIdentity(identityName)!;
        var templateCreator = ClientPool.FirstForIdentity(identityName == participant1 ? participant2 : participant1)!;

        var relationshipTemplateResponse = await CreateRelationshipTemplate(templateCreator);
        var createRelationshipResponse = await CreateRelationship(relationshipCreator, relationshipTemplateResponse.Result!.Id);

        _relationshipsContext.RelationshipMetadataObjects.Add(relationshipName, createRelationshipResponse.Result!);
    }

    [Given(@"a Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipRBetweenIAndI(string relationshipName, string identity1Name, string identity2Name)
    {
        var relationship = await Utils.EstablishRelationshipBetween(ClientPool.FirstForIdentity(identity1Name)!, ClientPool.FirstForIdentity(identity2Name)!);
        _relationshipsContext.Relationships[relationshipName] = relationship;
    }

    [Given(@"([a-zA-Z0-9]+) has terminated ([a-zA-Z0-9]+)")]
    public async Task GivenRIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationshipsContext.Relationships[relationshipName];
        var terminator = ClientPool.FirstForIdentity(terminatorName)!;

        _responseContext.TerminateRelationshipResponse = await terminator.Relationships.TerminateRelationship(relationship.Id);
        _responseContext.TerminateRelationshipResponse.Should().BeASuccess();
    }

    [Given(@"([a-zA-Z0-9]+) has decomposed ([a-zA-Z0-9]+)")]
    public async Task GivenIHasDecomposedItsRelationshipToI(string decomposerName, string relationshipName)
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
    public async Task WhenISendsAPostRequestToTheRelationshipsEndpointWithRtId(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.CreateRelationshipResponse = await CreateRelationship(ClientPool.FirstForIdentity(identityName)!, _responseContext.CreateRelationshipTemplateResponse!.Result!.Id);
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships/{([a-zA-Z0-9]+).Id}/Accept endpoint")]
    public async Task WhenISendsAPostRequestToTheRelationshipsRIdAcceptEndpoint(string identityName, string relationshipName)
    {
        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.AcceptRelationshipResponse = await ClientPool.FirstForIdentity(identityName)!.Relationships.AcceptRelationship(_relationshipsContext.RelationshipMetadataObjects[relationshipName].Id, acceptRelationshipRequest);
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships/{([a-zA-Z0-9]+).Id}/Reject endpoint")]
    public async Task WhenISendsAPostRequestToTheRelationshipsRIdRejectEndpoint(string identityName, string relationshipName)
    {
        var rejectRelationshipRequest = new RejectRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.RejectRelationshipResponse = await ClientPool.FirstForIdentity(identityName)!.Relationships.RejectRelationship(_relationshipsContext.RelationshipMetadataObjects[relationshipName].Id, rejectRelationshipRequest);
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Relationships/{([a-zA-Z0-9]+).Id}/Revoke endpoint")]
    public async Task WhenISendsAPostRequestToTheRelationshipsRIdRevokeEndpoint(string identityName, string relationshipName)
    {
        var revokeRelationshipRequest = new RevokeRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.RevokeRelationshipResponse = await ClientPool.FirstForIdentity(identityName)!.Relationships.RevokeRelationship(_relationshipsContext.RelationshipMetadataObjects[relationshipName].Id, revokeRelationshipRequest);
    }
    #endregion

    private static async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateRelationshipTemplate(Client client)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = "AAA".GetBytes()
        };

        return await client.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
    }

    private static async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(Client client, string relationshipTemplateId)
    {
        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateId,
            Content = "AAA".GetBytes()
        };

        return await client.Relationships.CreateRelationship(createRelationshipRequest);
    }
}

public class RelationshipsContext
{
    public readonly Dictionary<string, RelationshipMetadata> RelationshipMetadataObjects = new();
    public readonly Dictionary<string, Relationship> Relationships = new();
}
