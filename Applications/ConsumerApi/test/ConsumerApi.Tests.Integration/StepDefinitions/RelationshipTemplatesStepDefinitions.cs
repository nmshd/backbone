using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipTemplatesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public RelationshipTemplatesStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /RelationshipTemplates endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipTemplatesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES });
    }

    #endregion
}
