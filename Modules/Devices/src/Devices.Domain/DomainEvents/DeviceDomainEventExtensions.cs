using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.DomainEvents;

public static class BackupDeviceUsedDomainEventExtensions
{
    extension(BackupDeviceUsedDomainEvent)
    {
        public static BackupDeviceUsedDomainEvent Create(IdentityAddress identityAddress) => new() { IdentityAddress = identityAddress.Value };
    }
}

public static class FeatureFlagsOfIdentityChangedDomainEventExtensions
{
    extension(FeatureFlagsOfIdentityChangedDomainEvent)
    {
        public static FeatureFlagsOfIdentityChangedDomainEvent Create(Identity identity) => new() { IdentityAddress = identity.Address.Value };
    }
}

public static class IdentityCreatedDomainEventExtensions
{
    extension(IdentityCreatedDomainEvent)
    {
        public static IdentityCreatedDomainEvent Create(Identity identity) => new()
        {
            DomainEventId = $"{identity.Address}/Created",
            Address = identity.Address,
            Tier = identity.TierId
        };
    }
}

public static class IdentityDeletedDomainEventExtensions
{
    extension(IdentityDeletedDomainEvent)
    {
        public static IdentityDeletedDomainEvent Create(IdentityAddress identityAddress) => new()
        {
            DomainEventId = $"{identityAddress}/IdentityDeleted",
            IdentityAddress = identityAddress.Value
        };
    }
}

public static class IdentityDeletionCancelledDomainEventExtensions
{
    extension(IdentityDeletionCancelledDomainEvent)
    {
        public static IdentityDeletionCancelledDomainEvent Create(IdentityAddress identityAddress) => new()
        {
            DomainEventId = DomainEventIdFactory.Randomize($"{identityAddress}/IdentityDeletionCancelled"),
            IdentityAddress = identityAddress.Value
        };
    }
}

public static class IdentityDeletionProcessStartedDomainEventExtensions
{
    extension(IdentityDeletionProcessStartedDomainEvent)
    {
        public static IdentityDeletionProcessStartedDomainEvent Create(string identityAddress, string deletionProcessId, string? initiator) => new()
        {
            DomainEventId = $"{identityAddress}/DeletionProcessStarted/{deletionProcessId}",
            Address = identityAddress,
            DeletionProcessId = deletionProcessId,
            Initiator = initiator
        };
    }
}

public static class IdentityDeletionProcessStatusChangedDomainEventExtensions
{
    extension(IdentityDeletionProcessStatusChangedDomainEvent)
    {
        public static IdentityDeletionProcessStatusChangedDomainEvent Create(string deletionProcessOwner, string deletionProcessId, string? initiator) => new()
        {
            DomainEventId = $"{deletionProcessOwner}/IdentityDeletionProcessStatusChanged/{deletionProcessId}",
            DeletionProcessOwner = deletionProcessOwner,
            DeletionProcessId = deletionProcessId,
            Initiator = initiator
        };
    }
}

public static class IdentityToBeDeletedDomainEventExtensions
{
    extension(IdentityToBeDeletedDomainEvent)
    {
        public static IdentityToBeDeletedDomainEvent Create(IdentityAddress identityAddress, DateTime gracePeriodEndsAt) => new()
        {
            DomainEventId = DomainEventIdFactory.Randomize($"{identityAddress}/IdentityToBeDeleted"),
            IdentityAddress = identityAddress.Value,
            GracePeriodEndsAt = gracePeriodEndsAt
        };
    }
}

public static class TierCreatedDomainEventExtensions
{
    extension(TierCreatedDomainEvent)
    {
        public static TierCreatedDomainEvent Create(Tier tier) => new()
        {
            DomainEventId = $"{tier.Id}/Created",
            Id = tier.Id,
            Name = tier.Name
        };
    }
}

public static class TierDeletedDomainEventExtensions
{
    extension(TierDeletedDomainEvent)
    {
        public static TierDeletedDomainEvent Create(Tier tier) => new()
        {
            DomainEventId = $"{tier.Id.Value}/Deleted",
            Id = tier.Id
        };
    }
}

public static class TierOfIdentityChangedDomainEventExtensions
{
    extension(TierOfIdentityChangedDomainEvent)
    {
        public static TierOfIdentityChangedDomainEvent Create(Identity identity, TierId oldTierId, TierId newTierId) => new()
        {
            DomainEventId = DomainEventIdFactory.Randomize($"{identity.Address}/TierOfIdentityChanged"),
            OldTierId = oldTierId,
            NewTierId = newTierId,
            IdentityAddress = identity.Address
        };
    }
}

internal static class DomainEventIdFactory
{
    public static string Randomize(string domainEventId) => domainEventId + "/" + Guid.NewGuid().ToString("N")[..3];
}
