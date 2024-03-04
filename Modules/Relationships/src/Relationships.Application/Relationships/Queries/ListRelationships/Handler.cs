using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class Handler : IRequestHandler<ListRelationshipsQuery, ListRelationshipsResponse>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository)
    {
        _mapper = mapper;
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<ListRelationshipsResponse> Handle(ListRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipsRepository.FindRelationshipsWithIds(request.Ids, _userContext.GetAddress(), request.PaginationFilter, cancellationToken, track: false);

        return new ListRelationshipsResponse(_mapper.Map<RelationshipDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
