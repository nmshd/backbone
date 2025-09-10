using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Challenges.Domain.Entities;

public class Challenge : Entity
{
    private const int EXPIRY_TIME_IN_MINUTES = 10;

    public Challenge() : this(null, null)
    {
    }

    public Challenge(IdentityAddress? createdBy, DeviceId? createdByDevice)
    {
        Id = ChallengeId.New();
        ExpiresAt = SystemTime.UtcNow.AddMinutes(EXPIRY_TIME_IN_MINUTES);
        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
    }

    public ChallengeId Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public IdentityAddress? CreatedBy { get; set; }
    public DeviceId? CreatedByDevice { get; set; }

    public bool IsExpired()
    {
        return ExpiresAt <= SystemTime.UtcNow.AddHours(-1);
    }

    public static Expression<Func<Challenge, bool>> CanBeCleanedUp => c => c.ExpiresAt <= SystemTime.UtcNow.AddDays(-30);

    public static Expression<Func<Challenge, bool>> WasCreatedBy(string identityAddress)
    {
        return i => i.CreatedBy == identityAddress;
    }
}
