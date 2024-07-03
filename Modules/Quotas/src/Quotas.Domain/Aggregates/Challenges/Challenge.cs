using System.Linq.Expressions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Challenges;

public class Challenge
{
    public const int EXPIRY_TIME_IN_MINUTES = 10;

    public required string Id { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required string? CreatedBy { get; set; }

    public DateTime GetDateOfCreation()
    {
        return ExpiresAt.AddMinutes(-EXPIRY_TIME_IN_MINUTES);
    }

    public static Expression<Func<Challenge, bool>> WasCreatedInIntervalBy(DateTime from, DateTime to, string identityAddress)
    {
        return c => c.GetDateOfCreation() > from && c.GetDateOfCreation() < to && c.CreatedBy == identityAddress;
    }
}
