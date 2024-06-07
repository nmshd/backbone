﻿using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
public class IdentityDeletionCanceledDomainEvent : DomainEvent
{
    public IdentityDeletionCanceledDomainEvent(IdentityAddress identityAddress) : base($"{identityAddress}/IdentityDeletionCanceled", randomizeId: true)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; }
}
