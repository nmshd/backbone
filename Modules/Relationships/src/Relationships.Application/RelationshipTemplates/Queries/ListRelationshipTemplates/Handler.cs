using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

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
        var dbPaginationResult = await _relationshipTemplatesRepository.FindTemplatesWithIds(request.Ids.Select(RelationshipTemplateId.Parse), _userContext.GetAddress(), request.PaginationFilter,
            cancellationToken, track: false);

        return new ListRelationshipTemplatesResponse(dbPaginationResult.ItemsOnPage.Select(x => new RelationshipTemplateDTO(x)), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
