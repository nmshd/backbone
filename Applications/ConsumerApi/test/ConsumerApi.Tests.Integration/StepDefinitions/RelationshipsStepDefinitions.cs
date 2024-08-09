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
    private readonly IdentitiesContext _identitiesContext;
    private readonly RelationshipsContext _relationshipsContext;
    private readonly ResponseContext _responseContext;

    public RelationshipsStepDefinitions(IdentitiesContext identitiesContext, RelationshipsContext relationshipsContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _relationshipsContext = relationshipsContext;
        _responseContext = responseContext;
    }

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];
    private RelationshipMetadata RelationshipMetadata(string relationshipName) => _relationshipsContext.RelationshipMetadataObjects[relationshipName];
    private ApiResponse<CreateRelationshipTemplateResponse> CreateRelationshipTemplateResponse => _responseContext.CreateRelationshipTemplateResponse!;

    #region Given
    [Given("a Relationship Template rt created by ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipTemplateRtCreatedByI(string identityName)
    {
        _responseContext.CreateRelationshipTemplateResponse = await CreateRelationshipTemplate(Identity(identityName));
    }

    [Given("a pending Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenAPendingRelationshipBetweenI1AndI2CreatedByI2(string relationshipName, string participant1, string participant2, string identityName)
    {
        var relationshipCreator = Identity(identityName);
        var templateCreator = Identity(identityName == participant1 ? participant2 : participant1);

        var relationshipTemplateResponse = await CreateRelationshipTemplate(templateCreator);
        var createRelationshipResponse = await CreateRelationship(relationshipCreator, relationshipTemplateResponse.Result!.Id);

        _relationshipsContext.RelationshipMetadataObjects.Add(relationshipName, createRelationshipResponse.Result!);
    }

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
    #endregion

    #region When
    [When("a POST request is sent to the /Relationships endpoint by ([a-zA-Z0-9]+) with rt.id")]
    public async Task WhenAPostRequestIsSentToTheRelationshipsEndpointByIWith(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.CreateRelationshipResponse = await CreateRelationship(Identity(identityName), CreateRelationshipTemplateResponse.Result!.Id);
    }

    [When("a POST request is sent to the /Relationships/{([a-zA-Z0-9]+).Id}/Accept endpoint by ([a-zA-Z0-9]+)")]
    public async Task WhenAPostRequestIsSentToTheAcceptRelationshipEndpointByI(string relationshipName, string identityName)
    {
        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.AcceptRelationshipResponse = await Identity(identityName).Relationships.AcceptRelationship(RelationshipMetadata(relationshipName).Id, acceptRelationshipRequest);
    }

    [When("a POST request is sent to the /Relationships/{([a-zA-Z0-9]+).Id}/Reject endpoint by ([a-zA-Z0-9]+)")]
    public async Task WhenAPostRequestIsSentToTheRejectRelationshipEndpointByI(string relationshipName, string identityName)
    {
        var rejectRelationshipRequest = new RejectRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.RejectRelationshipResponse = await Identity(identityName).Relationships.RejectRelationship(RelationshipMetadata(relationshipName).Id, rejectRelationshipRequest);
    }

    [When("a POST request is sent to the /Relationships/{([a-zA-Z0-9]+).Id}/Revoke endpoint by ([a-zA-Z0-9]+)")]
    public async Task WhenAPostRequestIsSentToTheRevokeRelationshipEndpointByI(string relationshipName, string identityName)
    {
        var revokeRelationshipRequest = new RevokeRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };
        _responseContext.WhenResponse = _responseContext.RevokeRelationshipResponse = await Identity(identityName).Relationships.RevokeRelationship(RelationshipMetadata(relationshipName).Id, revokeRelationshipRequest);
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

public class RelationshipsContext
{
    public readonly Dictionary<string, RelationshipMetadata> RelationshipMetadataObjects = new();
    public readonly Dictionary<string, Relationship> Relationships = new();
}
