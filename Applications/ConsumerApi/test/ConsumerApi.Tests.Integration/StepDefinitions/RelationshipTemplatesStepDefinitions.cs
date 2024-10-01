using System.Text;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
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

    private ApiResponse<ListRelationshipTemplatesResponse>? _listRelationshipTemplatesResponse;

    public RelationshipTemplatesStepDefinitions(ResponseContext responseContext, RelationshipTemplatesContext relationshipTemplatesContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _relationshipTemplatesContext = relationshipTemplatesContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($@"Relationship Template {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING} with password ""([^""]*)"" and forIdentity {RegexFor.SINGLE_THING_OR_DEFAULT}")]
    public async Task GivenRelationshipTemplateCreatedByTokenOwnerWithPasswordAndForIdentity(string relationshipTemplateName, string identityName, string passwordString, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = forIdentityName != "-" ? _clientPool.FirstForIdentityName(forIdentityName).IdentityData!.Address : null;
        var password = passwordString.Trim() != "-" ? Convert.FromBase64String(passwordString.Trim()) : null;

        var response = await client.RelationshipTemplates
            .CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient, Password = password });

        _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName] = response.Result!;
    }

    [Given(@"Relationship Templates with the following properties")]
    public async Task GivenRelationshipTemplatesWithTheFollowingProperties(Table table)
    {
        for (var i = 0; i < table.RowCount; i++)
        {
            var (relationshipTemplateName, identityName, forIdentityName, passwordString) = ExtractRelationshipTemplateCreationValues(table.Rows[i]);

            var client = _clientPool.FirstForIdentityName(identityName);
            var forClient = forIdentityName != "-" ? _clientPool.FirstForIdentityName(forIdentityName).IdentityData!.Address : null;
            var password = passwordString.Trim() != "-" ? Convert.FromBase64String(passwordString.Trim()) : null;

            var response = await client.RelationshipTemplates
                .CreateTemplate(new CreateRelationshipTemplateRequest { Content = TestData.SOME_BYTES, ForIdentity = forClient, Password = password });

            _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName] = response.Result!;
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
            ? await client.RelationshipTemplates.GetTemplate(relationshipTemplateId, password.Trim())
            : await client.RelationshipTemplates.GetTemplate(relationshipTemplateId);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /RelationshipTemplate endpoint with the following payloads")]
    public async Task WhenISendsAGETRequestToTheRelationshipTemplateEndpointWithTheFollowingPayloads(string identityName, Table table)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var jsonTemplate = "[";

        for (var i = 0; i < table.RowCount; i++)
        {
            var (rTempName, passwordOnGet) = ExtractRelationshipTemplateQueryValues(table.Rows[0]);
            var relationshipTemplateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[rTempName].Id;
            var q = $"{{\"Id\":\"{relationshipTemplateId}\"{(passwordOnGet == "-" ? "" : $",\"Password\":\"{passwordOnGet}\"")}}}";

            if (i < table.RowCount - 1)
                q += ",";

            jsonTemplate += q;
        }

        jsonTemplate += "]";

        _responseContext.WhenResponse = _listRelationshipTemplatesResponse = await client.RelationshipTemplates.ListTemplates(jsonTemplate);
    }

    #endregion

    #region Then

    [Then(@"the response contains (\d+) Relationship Template\(s\)")]
    public void ThenTheResponseContainsRelationshipTemplates(int relationshipTemplateCount)
    {
        _listRelationshipTemplatesResponse!.Result!.Count.Should().Be(relationshipTemplateCount);
    }

    #endregion

    private static (string name, string rTempOwner, string forIdentity, string password) ExtractRelationshipTemplateCreationValues(TableRow row)
    {
        return (
            row.TryGetValue("rTempName", out var rTempName) ? rTempName : string.Empty,
            row.TryGetValue("rTempOwner", out var rTempOwner) ? rTempOwner : string.Empty,
            row.TryGetValue("forIdentity", out var forIdentity) ? forIdentity : string.Empty,
            row.TryGetValue("password", out var password) ? password : string.Empty
        );
    }

    private static (string rTempName, string passwordOnGet) ExtractRelationshipTemplateQueryValues(TableRow row)
    {
        return (
            row.TryGetValue("rTempName", out var rTempName) ? rTempName : string.Empty,
            row.TryGetValue("passwordOnGet", out var passwordOnGet) ? passwordOnGet : string.Empty
        );
    }
}
