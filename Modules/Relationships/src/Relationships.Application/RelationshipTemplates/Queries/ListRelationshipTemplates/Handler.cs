using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class Handler : IRequestHandler<ListRelationshipTemplatesQuery, ListRelationshipTemplatesResponse>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _mapper = mapper;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<ListRelationshipTemplatesResponse> Handle(ListRelationshipTemplatesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipTemplatesRepository.FindTemplatesWithIds(request.Ids, _userContext.GetAddress(), request.PaginationFilter, cancellationToken, track: false);

        return new ListRelationshipTemplatesResponse(_mapper.Map<RelationshipTemplateDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
