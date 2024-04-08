using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities;

public class Challenge
{
    // ReSharper disable once UnusedMember.Local
    private Challenge()
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
