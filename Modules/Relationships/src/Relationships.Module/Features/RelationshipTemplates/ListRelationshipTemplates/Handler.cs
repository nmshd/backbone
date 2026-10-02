using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipTemplatesRepository.ListWithContent(request.Ids, _userContext.GetAddress(), request.PaginationFilter,
            cancellationToken, track: false);

        return new Response(dbPaginationResult, request.PaginationFilter);
    }
}
