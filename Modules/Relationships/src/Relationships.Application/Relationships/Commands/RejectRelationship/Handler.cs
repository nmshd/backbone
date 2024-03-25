using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class Handler : IRequestHandler<RejectRelationshipCommand, RejectRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IRelationshipsRepository relationshipsRepository, IUserContext userContext, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<RejectRelationshipResponse> Handle(RejectRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(request.RelationshipId, _activeIdentity, cancellationToken, track: true) ??
                           throw new NotFoundException(nameof(Relationship));

        relationship.Reject(_activeIdentity, _activeDevice);

        await _relationshipsRepository.Update(relationship);

        _eventBus.Publish(new RelationshipStatusChangedIntegrationEvent(relationship));

        return new RejectRelationshipResponse(relationship);
    }
}
