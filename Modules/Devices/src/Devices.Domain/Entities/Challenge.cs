using Backbone.BuildingBlocks.Domain;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities;

public class Challenge : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected Challenge()
    {
        Id = null!;
    }

    public string Id { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsExpired()
    {
        return ExpiresAt < SystemTime.UtcNow;
    }
}
