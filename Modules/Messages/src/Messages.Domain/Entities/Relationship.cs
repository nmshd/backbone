using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Ids;

namespace Messages.Domain.Entities;

public class Relationship
{
#pragma warning disable CS8618
    private Relationship() { }
#pragma warning restore CS8618

    public RelationshipId Id { get; }

    public IdentityAddress From { get; }
    public IdentityAddress To { get; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; private set; }
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
