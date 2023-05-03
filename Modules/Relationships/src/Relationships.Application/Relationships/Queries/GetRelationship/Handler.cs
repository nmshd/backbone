using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class Handler : RequestHandlerBase<GetRelationshipQuery, RelationshipDTO>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository) : base(dbContext, userContext, mapper)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public override async Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(request.Id, _activeIdentity, cancellationToken, track: false);

        return _mapper.Map<RelationshipDTO>(relationship);
    }
}
