using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Relationship
{
    // ReSharper disable once UnusedMember.Local
    private Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        From = null!;
        To = null!;
        CreatedAt = default;
        Status = default;
    }

    public RelationshipId Id { get; internal set; }

    public IdentityAddress From { get; internal set; }
    public IdentityAddress To { get; internal set; }

    public DateTime CreatedAt { get; internal set; }

    public RelationshipStatus Status { get; internal set; }

    public void EnsureSendingMessagesIsAllowed(int numberOfUnreceivedMessagesFromActiveIdentity, int maxNumberOfUnreceivedMessagesFromOneSender)
    {
        if (Status != RelationshipStatus.Active)
            throw new DomainException(DomainErrors.RelationshipToRecipientNotActive(To));

        if (numberOfUnreceivedMessagesFromActiveIdentity >= maxNumberOfUnreceivedMessagesFromOneSender)
            throw new DomainException(DomainErrors.MaxNumberOfUnreceivedMessagesReached(To));
    }

    public static Relationship LoadForTesting(RelationshipId id, IdentityAddress from, IdentityAddress to, DateTime createdAt, RelationshipStatus status)
    {
        return new Relationship
        {
            Id = id,
            From = from,
            To = to,
            CreatedAt = createdAt,
            Status = status
        };
    }
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminating = 50,
    Terminated = 60
}
