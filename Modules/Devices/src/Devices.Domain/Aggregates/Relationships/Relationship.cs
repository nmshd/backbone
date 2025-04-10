using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Relationships;

public class Relationship : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public string Id { get; } = null!;

    public IdentityAddress From { get; } = null!;
    public IdentityAddress To { get; } = null!;

    public static Expression<Func<Relationship, bool>> IsBetween(IdentityAddress address1, IdentityAddress address2)
    {
        return r => (r.From == address1 && r.To == address2) || (r.From == address2 && r.To == address1);
    }
}
