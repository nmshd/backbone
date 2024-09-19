﻿using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}
