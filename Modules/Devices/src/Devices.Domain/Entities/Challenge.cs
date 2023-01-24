using Enmeshed.Tooling;

namespace Devices.Domain.Entities;

public class Challenge
{
#pragma warning disable CS8618
    private Challenge() { }
#pragma warning restore CS8618

    public string Id { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsExpired()
    {
        return ExpiresAt < SystemTime.UtcNow;
    }
}
