using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.TerminateRelationship;

public class Handler : IRequestHandler<Command, RelationshipMetadataDTO>
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

    public async Task<RelationshipMetadataDTO> Handle(Command request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.GetRelationshipWithoutContent(relationshipId, _activeIdentity, cancellationToken, track: true);

        relationship.Terminate(_activeIdentity, _activeDevice);

        await _relationshipsRepository.Update(relationship);

        return new RelationshipMetadataDTO(relationship);
    }
}
