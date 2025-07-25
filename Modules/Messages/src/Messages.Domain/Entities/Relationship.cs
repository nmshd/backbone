using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Relationship : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        From = null!;
        To = null!;
        CreatedAt = default;
        Status = default;
    }

    private Relationship(RelationshipId id, IdentityAddress from, IdentityAddress to, DateTime createdAt, RelationshipStatus status)
    {
        Id = id;
        From = from;
        To = to;
        CreatedAt = createdAt;
        Status = status;
    }

    public RelationshipId Id { get; }

    public IdentityAddress From { get; }
    public IdentityAddress To { get; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; }

    public void EnsureSendingMessagesIsAllowed(IdentityAddress activeIdentity, int numberOfUnreceivedMessagesFromActiveIdentity, int maxNumberOfUnreceivedMessagesFromOneSender)
    {
        if (Status is not (RelationshipStatus.Active or RelationshipStatus.Pending or RelationshipStatus.Terminated))
            throw new DomainException(DomainErrors.RelationshipToRecipientNotActive(GetPeerOf(activeIdentity)));

        if (numberOfUnreceivedMessagesFromActiveIdentity >= maxNumberOfUnreceivedMessagesFromOneSender)
            throw new DomainException(DomainErrors.MaxNumberOfUnreceivedMessagesReached(To));
    }

    public static Relationship LoadForTesting(RelationshipId id, IdentityAddress from, IdentityAddress to, DateTime createdAt, RelationshipStatus status)
    {
        return new Relationship(id, from, to, createdAt, status);
    }

    private IdentityAddress GetPeerOf(IdentityAddress activeIdentity)
    {
        return From == activeIdentity ? To : From;
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
