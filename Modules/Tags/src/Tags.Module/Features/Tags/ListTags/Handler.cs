using Backbone.Modules.Tags.Abstractions;
using MediatR;

namespace Backbone.Modules.Tags.Module.Features.Tags.ListTags;

public class Handler : IRequestHandler<ListTagsQuery, ListTagsResponse>
{
    private readonly ITagsRepository _tagsRepository;

    public Handler(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public Task<ListTagsResponse> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {
        var response = new ListTagsResponse
        {
            SupportedLanguages = _tagsRepository.ListSupportedLanguages(),
            TagsForAttributeValueTypes = _tagsRepository.ListAttributes()
        };

        return Task.FromResult(response);
    }
}
