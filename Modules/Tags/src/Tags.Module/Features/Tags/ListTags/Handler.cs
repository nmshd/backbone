using Backbone.Modules.Tags.Abstractions;
using MediatR;

namespace Backbone.Modules.Tags.Module.Features.Tags.ListTags;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly ITagsRepository _tagsRepository;

    public Handler(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var response = new Response
        {
            SupportedLanguages = _tagsRepository.ListSupportedLanguages(),
            TagsForAttributeValueTypes = _tagsRepository.ListAttributes()
        };

        return Task.FromResult(response);
    }
}
