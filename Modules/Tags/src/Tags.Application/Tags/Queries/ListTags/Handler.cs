using MediatR;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

public class Handler : IRequestHandler<ListTagsQuery, ListTagsResponse>
{
    private readonly TagProvider _tagProvider;

    public Handler(TagProvider tagProvider)
    {
        _tagProvider = tagProvider;
    }

    public Task<ListTagsResponse> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {
        var response = new ListTagsResponse(_tagProvider.LegalTags);

        return Task.FromResult(response);
    }
}
