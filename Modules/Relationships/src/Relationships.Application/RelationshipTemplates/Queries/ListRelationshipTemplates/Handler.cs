using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class Handler : RequestHandlerBase<ListRelationshipTemplatesQuery, ListRelationshipTemplatesResponse>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IRelationshipTemplatesRepository relationshipTemplatesRepository) : base(dbContext, userContext, mapper)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
    }

    public override async Task<ListRelationshipTemplatesResponse> Handle(ListRelationshipTemplatesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipTemplatesRepository.FindTemplatesWithIds(request.Ids, _activeIdentity, request.PaginationFilter, track: true);

        return new ListRelationshipTemplatesResponse(_mapper.Map<RelationshipTemplateDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
