using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class Handler : IRequestHandler<RejectRelationshipCommand, RejectRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IRelationshipsRepository relationshipsRepository, IUserContext userContext)
    {
        _relationshipsRepository = relationshipsRepository;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<RejectRelationshipResponse> Handle(RejectRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);

        var relationship = await _relationshipsRepository.GetRelationshipWithContent(relationshipId, _activeIdentity, cancellationToken, track: true) ??
                           throw new NotFoundException(nameof(Relationship));

        relationship.Reject(_activeIdentity, _activeDevice, request.CreationResponseContent);

        await _relationshipsRepository.Update(relationship);

        return new RejectRelationshipResponse(relationship);
    }
}
