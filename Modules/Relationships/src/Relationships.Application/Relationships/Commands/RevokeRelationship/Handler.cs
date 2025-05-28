using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;

public class Handler : IRequestHandler<RevokeRelationshipCommand, RevokeRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IdentityAddress _activeIdentity;
    private readonly DeviceId _activeDevice;

    public Handler(IRelationshipsRepository relationshipsRepository, IUserContext userContext)
    {
        _relationshipsRepository = relationshipsRepository;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<RevokeRelationshipResponse> Handle(RevokeRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.GetRelationship(relationshipId, _activeIdentity, cancellationToken, track: true);

        relationship.Revoke(_activeIdentity, _activeDevice, request.CreationResponseContent);

        await _relationshipsRepository.Update(relationship);

        return new RevokeRelationshipResponse(relationship);
    }
}
