using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
public class TagsStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<ListTagsResponse>? _listTagsResponse;

    public TagsStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    [When("A GET request to the /Tags endpoint gets sent")]
    public async Task WhenAGETRequestToTheTagsEndpointGetsSent()
    {
        var client = _clientPool.Anonymous;
        _responseContext.WhenResponse = _listTagsResponse = await client.Tags.ListTags();
    }

    [Then("the response supports the English language")]
    public void AndTheResponseSupportsTheEnglishLanguage()
    {
        _listTagsResponse!.Result!.SupportedLanguages.Should().Contain("en");
    }

    [Then("the response attributes contain tags")]
    public void AndTheResponseAttributesContainTags()
    {
        foreach (var attr in _listTagsResponse!.Result!.TagsForAttributeValueTypes.Values)
        {
            attr.Should().NotBeEmpty();
        }
    }
}
