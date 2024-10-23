using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class Handler : IRequestHandler<GetRelationshipQuery, RelationshipDTO>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(RelationshipId.Parse(request.Id), _userContext.GetAddress(), cancellationToken, track: false);

        return new RelationshipDTO(relationship);
    }
}
