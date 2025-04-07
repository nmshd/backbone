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
}
