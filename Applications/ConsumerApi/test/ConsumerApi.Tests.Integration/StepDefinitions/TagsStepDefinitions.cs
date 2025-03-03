using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
public class TagsStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private CachedApiResponse<ListTagsResponse>? _listTagsResponse;

    public TagsStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #region Given

    [Given($@"a list of tags {RegexFor.SINGLE_THING} with an ETag {RegexFor.SINGLE_THING}")]
    public async Task GivenAListOfTagsWithETag(string list, string hash)
    {
        await WhenAGETRequestToTheTagsEndpointGetsSent();

        _listTagsResponse!.Should().BeASuccess();
    }

    [Given($@"{RegexFor.SINGLE_THING} didn't change since the last fetch")]
    public void GivenListDidntChangeSinceLastFetch(string list)
    {
    }

    #endregion


    #region When

    [When("A GET request to the /Tags endpoint gets sent")]
    public async Task WhenAGETRequestToTheTagsEndpointGetsSent()
    {
        var client = _clientPool.Anonymous;
        _responseContext.WhenResponse = _listTagsResponse = await client.Tags.ListTags();
    }

    [When($@"A GET request to the /Tags endpoint gets sent with the If-None-Match header set to {RegexFor.SINGLE_THING}")]
    public async Task WhenAGETRequestToTheTagsEndpointGetsSentWithHash(string hash)
    {
        var client = _clientPool.Anonymous;
        _responseContext.WhenResponse = _listTagsResponse = await client.Tags.ListTags(new CacheControl { ETag = _listTagsResponse!.ETag });
    }

    #endregion

    #region Then

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

    [Then("the response content is empty")]
    public void ThenTheResponseContentIsEmpty()
    {
        _listTagsResponse!.NotModified.Should().BeTrue();
        _listTagsResponse!.Result.Should().BeNull();
    }

    #endregion
}
