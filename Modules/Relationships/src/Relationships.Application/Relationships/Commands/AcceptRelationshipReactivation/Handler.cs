﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;
public class Handler : IRequestHandler<AcceptRelationshipReactivationCommand, AcceptRelationshipReactivationResponse>
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

    public async Task<AcceptRelationshipReactivationResponse> Handle(AcceptRelationshipReactivationCommand request, CancellationToken cancellationToken)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.FindRelationship(relationshipId, _activeIdentity, cancellationToken, track: true);

        relationship.AcceptReactivation(_activeIdentity, _activeDevice);

        await _relationshipsRepository.Update(relationship);

        _eventBus.Publish(new RelationshipReactivationCompletedDomainEvent(relationship, _activeIdentity));

        return new AcceptRelationshipReactivationResponse(relationship);
    }
}
