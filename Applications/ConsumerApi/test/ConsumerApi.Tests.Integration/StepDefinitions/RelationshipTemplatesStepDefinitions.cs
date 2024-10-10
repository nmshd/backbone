using System.Text;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using TechTalk.SpecFlow.Assist;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class RelationshipTemplatesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;
    private readonly RelationshipTemplatesContext _relationshipTemplatesContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<ListRelationshipTemplatesResponse>? _listRelationshipTemplatesResponse;

    public RelationshipTemplatesStepDefinitions(ResponseContext responseContext, RelationshipTemplatesContext relationshipTemplatesContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _relationshipTemplatesContext = relationshipTemplatesContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($@"Relationship Template {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING} with password ""([^""]*)"" and forIdentity {RegexFor.OPTIONAL_SINGLE_THING}")]
    public async Task GivenRelationshipTemplateCreatedByTokenOwnerWithPasswordAndForIdentity(string relationshipTemplateName, string identityName, string passwordString, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = forIdentityName != "-" ? _clientPool.FirstForIdentityName(forIdentityName).IdentityData!.Address : null;
        var password = passwordString.Trim() != "-" ? Convert.FromBase64String(passwordString.Trim()) : null;

        var response = await client.RelationshipTemplates
            .CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient, Password = password });

        _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName] = response.Result!;
    }

    [Given(@"the following Relationship Templates")]
    public async Task GivenRelationshipTemplatesWithTheFollowingProperties(Table table)
    {
        var relationshipTemplatePropertiesSet = table.CreateSet<RelationshipTemplateProperties>();

        foreach (var relationshipTemplateProperties in relationshipTemplatePropertiesSet)
        {
            var client = _clientPool.FirstForIdentityName(relationshipTemplateProperties.RTempOwner);
            var forClient = relationshipTemplateProperties.ForIdentity != "-" ? _clientPool.FirstForIdentityName(relationshipTemplateProperties.ForIdentity).IdentityData!.Address : null;
            var password = relationshipTemplateProperties.Password.Trim() != "-" ? Convert.FromBase64String(relationshipTemplateProperties.Password.Trim()) : null;

            var response = await client.RelationshipTemplates
                .CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient, Password = password });

            _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateProperties.RTempName] = response.Result!;
        }
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /RelationshipTemplates endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipTemplatesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.RelationshipTemplates.CreateTemplate(
            new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES });
    }

    [When($@"{RegexFor.SINGLE_THING} sends a POST request to the /RelationshipTemplates endpoint with the password ""(.*)""")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipTemplatesEndpointWithThePassword(string identityName, string passwordString)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var password = Encoding.UTF8.GetBytes(passwordString);

        _responseContext.WhenResponse = await client.RelationshipTemplates.CreateTemplate(
            new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, Password = password });
    }

    [When($@"{RegexFor.SINGLE_THING} sends a POST request to the /RelationshipTemplate endpoint with password ""(.*)"" and forIdentity {RegexFor.SINGLE_THING}")]
    public async Task WhenIdentitySendsAPostRequestToTheRelationshipTemplatesEndpointWithPasswordAndForIdentity(string identityName, string passwordString, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = _clientPool.FirstForIdentityName(forIdentityName);
        var password = Encoding.UTF8.GetBytes(passwordString);

        _responseContext.WhenResponse = await client.RelationshipTemplates.CreateTemplate(
            new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient.IdentityData!.Address, Password = password });
    }

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /RelationshipTemplate/{RegexFor.SINGLE_THING}.Id endpoint with password ""([^""]*)""")]
    public async Task WhenIdentitySendsAGetRequestToTheRelationshipTemplatesIdEndpointWithPassword(string identityName, string relationshipTemplateName, string password)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var relationshipTemplateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName].Id;

        _responseContext.WhenResponse = password != "-"
            ? await client.RelationshipTemplates.GetTemplate(relationshipTemplateId, Convert.FromBase64String(password.Trim()))
            : await client.RelationshipTemplates.GetTemplate(relationshipTemplateId);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /RelationshipTemplate endpoint with the following payloads")]
    public async Task WhenISendsAGETRequestToTheRelationshipTemplateEndpointWithTheFollowingPayloads(string identityName, Table table)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var getRequestPayloadSet = table.CreateSet<GetRequestPayload>();

        var queries = getRequestPayloadSet.Select(payload =>
        {
            var relationshipTemplateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[payload.RTempName].Id;
            var password = payload.PasswordOnGet == "-" ? null : Convert.FromBase64String(payload.PasswordOnGet.Trim());

            return new RelationshipTemplateQuery { Id = relationshipTemplateId, Password = password };
        }).ToList();

        _responseContext.WhenResponse = _listRelationshipTemplatesResponse = await client.RelationshipTemplates.ListTemplates(queries);
    }

    #endregion

    #region Then

    [Then($@"the response contains Relationship Template\(s\) {RegexFor.LIST_OF_THINGS}")]
    public void ThenTheResponseContainsRelationshipTemplates(string relationshipTemplateNames)
    {
        var relationshipTemplates = relationshipTemplateNames.Split(',').Select(item => _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[item.Trim()]).ToList();
        _listRelationshipTemplatesResponse!.Result!.Should().BeEquivalentTo(relationshipTemplates, options => options.WithStrictOrdering());
    }

    #endregion
}

file class RelationshipTemplateProperties
{
    public required string RTempName { get; set; }
    public required string RTempOwner { get; set; }
    public required string ForIdentity { get; set; }
    public required string Password { get; set; }
}

file class GetRequestPayload
{
    public required string RTempName { get; set; }
    public required string PasswordOnGet { get; set; }
}
