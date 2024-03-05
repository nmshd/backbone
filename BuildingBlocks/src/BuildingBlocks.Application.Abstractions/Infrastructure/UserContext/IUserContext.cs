using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

public interface IUserContext
{
    IdentityAddress GetAddress();
    IdentityAddress? GetAddressOrNull();

    DeviceId GetDeviceId();
    DeviceId? GetDeviceIdOrNull();

    string GetUserId();
    string? GetUserIdOrNull();

    string GetUsername();
    string? GetUsernameOrNull();

    IEnumerable<string> GetRoles();
    SubscriptionPlan GetSubscriptionPlan();
}

public enum SubscriptionPlan
{
    Undefined,
    Free,
    Paid
}

public static class Roles
{
    public const string ADMIN = "admin";
}

public static class IUserContextExtensions
{
    public static bool IsAdmin(this IUserContext activeIdentity)
    {
        return activeIdentity.GetRoles().Any(r => r == Roles.ADMIN);
    }
}

public static class UserTypeExtensions
{
    public static bool IsPaid(this SubscriptionPlan subscriptionPlan)
    {
        return subscriptionPlan == SubscriptionPlan.Paid;
    }

    public static bool IsFree(this SubscriptionPlan subscriptionPlan)
    {
        return subscriptionPlan == SubscriptionPlan.Free;
    }

    public static bool IsUndefined(this SubscriptionPlan subscriptionPlan)
    {
        return subscriptionPlan == SubscriptionPlan.Undefined;
    }
}
