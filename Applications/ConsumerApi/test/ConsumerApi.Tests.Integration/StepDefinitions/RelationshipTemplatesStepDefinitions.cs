using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipTemplatesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;
    private readonly RelationshipTemplatesContext _relationshipTemplatesContext;
    private readonly ClientPool _clientPool;

    public RelationshipTemplatesStepDefinitions(ResponseContext responseContext, RelationshipTemplatesContext relationshipTemplatesContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _relationshipTemplatesContext = relationshipTemplatesContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"Relationship Template {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING} where ForIdentity is the address of {RegexFor.SINGLE_THING}")]
    public async Task GivenRelationshipTemplateCreatedByIWhereForIdentityIsTheAddressOfI(string relationshipTemplateName, string identityName, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = _clientPool.FirstForIdentityName(forIdentityName);

        var response = await client.RelationshipTemplates
            .CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient.IdentityData!.Address });

        _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName] = response.Result!;
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /RelationshipTemplates endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipTemplatesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES });
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /RelationshipTemplates/{{id}} endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenISendsAGetRequestToTheRelationshipTemplatesIdEndpointWithId(string identityName, string relationshipTemplateName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var relationshipTemplateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName].Id;

        _responseContext.WhenResponse = await client.RelationshipTemplates.GetTemplate(relationshipTemplateId);
    }


    #endregion
}
