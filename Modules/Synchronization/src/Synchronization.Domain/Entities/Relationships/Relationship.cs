using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Relationships;

public class Relationship
{
    public Relationship(RelationshipId id, IdentityAddress from, IdentityAddress to, RelationshipStatus status)
    {
        Id = id;
        From = from;
        To = to;
        Status = status;
    }

    public RelationshipId Id { get; } = null!;
    public IdentityAddress From { get; } = null!;
    public IdentityAddress To { get; } = null!;
    public RelationshipStatus Status { get; }

    public bool IsBetween(IdentityAddress identity1, IdentityAddress identity2)
    {
        return From == identity1 && To == identity2 ||
               From == identity2 && To == identity1;
    }
}
