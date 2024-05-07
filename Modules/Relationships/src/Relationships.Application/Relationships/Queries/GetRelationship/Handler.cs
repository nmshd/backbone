using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class Handler : IRequestHandler<GetRelationshipQuery, RelationshipDTO>
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

    public async Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(request.Id, _userContext.GetAddress(), cancellationToken, track: false);

        return new RelationshipDTO(relationship);
    }
}
