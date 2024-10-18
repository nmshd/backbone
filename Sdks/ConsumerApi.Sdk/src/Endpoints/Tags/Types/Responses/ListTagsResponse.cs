namespace Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types.Responses;

public class ListTagsResponse
{
    public required List<string> SupportedLanguages { get; set; }
    public required Dictionary<string, TagDefinition> TagsForAttributeValueTypes { get; set; }
}
