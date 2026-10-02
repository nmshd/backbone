using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetRelationship;

public class Handler : IRequestHandler<GetRelationshipQuery, RelationshipDTO>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.GetRelationshipWithContent(RelationshipId.Parse(request.Id), _activeIdentity, cancellationToken, track: false);

        relationship.EnsureIsNotDecomposedBy(_activeIdentity);

        return new RelationshipDTO(relationship);
    }
}
