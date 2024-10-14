﻿using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStatusChangedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcessStatusChangedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public IdentityDeletionProcessStatusChangedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.IdentityDeletionProcessStatusChanged, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string DeletionProcessId { get; init; }
    }
}
