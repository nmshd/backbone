using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

public class Handler : IRequestHandler<ListRelationshipTemplatesQuery, ListRelationshipTemplatesResponse>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<ListRelationshipTemplatesResponse> Handle(ListRelationshipTemplatesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipTemplatesRepository.ListWithContent(request.QueryItems, _userContext.GetAddress(), request.PaginationFilter,
            cancellationToken, track: false);

        return new ListRelationshipTemplatesResponse(dbPaginationResult, request.PaginationFilter);
    }
}
