using Backbone.Modules.Tags.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

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
            SupportedLanguages = _tagsRepository.GetSupportedLanguages(),
            TagsForAttributeValueTypes = _tagsRepository.GetAttributes()
        };

        return Task.FromResult(response);
    }
}
