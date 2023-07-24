using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
public class Relationship
{
    public string Id { get; set; }
    public IdentityAddress From { get; }
    public IdentityAddress To { get; }
}
