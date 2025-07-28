using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Relationships;

public class Relationship : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public string Id { get; } = null!;

    public IdentityAddress From { get; } = null!;
    public IdentityAddress To { get; } = null!;

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; }

    public void EnsureSendingNotificationsIsAllowed()
    {
        if (Status is not (RelationshipStatus.Pending or RelationshipStatus.Active or RelationshipStatus.Terminated))
            throw new DomainException(DomainErrors.RelationshipToRecipientIsNotInCorrectStatus([RelationshipStatus.Active, RelationshipStatus.Pending, RelationshipStatus.Terminated]));
    }

    public bool HasParticipant(IdentityAddress potentialPeerAddress)
    {
        return From == potentialPeerAddress || To == potentialPeerAddress;
    }

    public static Expression<Func<Relationship, bool>> IsBetween(IdentityAddress address1, IdentityAddress address2)
    {
        return r => (r.From == address1 && r.To == address2) || (r.From == address2 && r.To == address1);
    }

    public static Expression<Func<Relationship, bool>> IsBetween(IdentityAddress mainAddress, IdentityAddress[] peerAddresses)
    {
        return r => (r.From == mainAddress && peerAddresses.Contains(r.To)) || peerAddresses.Contains(r.From) && r.To == mainAddress;
    }
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60,
    ReadyForDeletion = 70
}
