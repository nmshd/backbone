using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ReactivateRelationship;
public class Handler : IRequestHandler<ReactivateRelationshipCommand, ReactivateRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;
    private readonly IdentityAddress _activeIdentity;
    private readonly DeviceId _activeDevice;

    public Handler(IRelationshipsRepository relationshipsRepository, IUserContext userContext, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }
    public async Task<ReactivateRelationshipResponse> Handle(ReactivateRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.FindRelationship(relationshipId, _activeIdentity, cancellationToken, track: true);

        relationship.Reactivate(_activeIdentity, _activeDevice);

        await _relationshipsRepository.Update(relationship);

        var partnerIdentity = relationship.To == _activeIdentity ? relationship.From : relationship.To;

        _eventBus.Publish(new RelationshipStatusChangedIntegrationEvent(relationship));
        _eventBus.Publish(new RelationshipReactivatedIntegrationEvent(relationship, partnerIdentity));

        return new ReactivateRelationshipResponse(relationship);
    }
}
