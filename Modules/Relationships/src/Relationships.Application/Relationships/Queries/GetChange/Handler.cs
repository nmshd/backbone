using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Queries.GetChange;

public class Handler : IRequestHandler<GetChangeQuery, RelationshipChangeDTO>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<RelationshipChangeDTO> Handle(GetChangeQuery query, CancellationToken cancellationToken)
    {
        var change = await _relationshipsRepository.FindRelationshipChange(query.Id, _userContext.GetAddress(), cancellationToken, track: false);

        return _mapper.Map<RelationshipChangeDTO>(change);
    }
}
