using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class BackupDeviceUsedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

public class IdentityCreatedDomainEvent : DomainEvent
{
    public required string Address { get; set; }
    public required string Tier { get; set; }
}

public class IdentityDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

public class IdentityDeletionCancelledDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

public class IdentityDeletionProcessStartedDomainEvent : DomainEvent
{
    public required string Address { get; set; }
    public required string DeletionProcessId { get; set; }
    public string? Initiator { get; set; }
}

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public required string DeletionProcessOwner { get; set; }
    public required string DeletionProcessId { get; set; }
    public string? Initiator { get; set; }
}

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
    public required DateTime GracePeriodEndsAt { get; set; }
}

public class TierCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required string Name { get; set; }
}

public class TierDeletedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
}

public class TierOfIdentityChangedDomainEvent : DomainEvent
{
    public required string OldTierId { get; set; }
    public required string NewTierId { get; set; }
    public required string IdentityAddress { get; set; }
}
