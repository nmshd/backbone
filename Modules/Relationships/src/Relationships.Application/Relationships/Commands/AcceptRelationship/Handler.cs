using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;

public class Handler : IRequestHandler<AcceptRelationshipCommand, AcceptRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IdentityAddress _activeIdentity;
    private readonly DeviceId _activeDevice;

    private RelationshipTemplate _template;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
        _relationshipsRepository = relationshipsRepository;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;

        _template = null!;
    }

    public async Task<AcceptRelationshipResponse> Handle(AcceptRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.FindRelationship(relationshipId, _activeIdentity, cancellationToken, track: true) ??
                           throw new NotFoundException(nameof(Relationship));

        await ReadTemplateFromDb(cancellationToken, relationship);

        var existingRelationships = await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(_activeIdentity), cancellationToken);

        relationship.Accept(_activeIdentity, _activeDevice, request.CreationResponseContent, existingRelationships.ToList());

        await _relationshipsRepository.Update(relationship);

        return new AcceptRelationshipResponse(relationship);
    }

    private async Task ReadTemplateFromDb(CancellationToken cancellationToken, Relationship relationship)
    {
        var templateId = relationship.RelationshipTemplateId;

        _template = await _relationshipTemplatesRepository.Find(templateId, _activeIdentity, cancellationToken, track: true) ??
                    throw new NotFoundException(nameof(RelationshipTemplate));
    }
}
