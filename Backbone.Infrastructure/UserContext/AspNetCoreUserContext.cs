using System.ComponentModel;
using System.Security.Claims;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Backbone.Infrastructure.UserContext;

public class AspNetCoreUserContext : IUserContext
{
    private const string ADDRESS_CLAIM = "address";
    private const string DEVICE_ID_CLAIM = "device_id";
    private const string USER_ID_CLAIM = "sub";
    private const string SUBSCRIPTION_PLAN_CLAIM = "subscription_plan";
    private readonly IHttpContextAccessor _context;

    public AspNetCoreUserContext(IHttpContextAccessor context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IdentityAddress GetAddress()
    {
        var address = _context.HttpContext.User.FindFirstValue(ADDRESS_CLAIM);

        if (address == null) throw new NotFoundException();

        return IdentityAddress.Parse(address);
    }

    public IdentityAddress? GetAddressOrNull()
    {
        var address = _context.HttpContext.User.FindFirstValue(ADDRESS_CLAIM);

        return address == null ? null : IdentityAddress.Parse(address);
    }

    public DeviceId GetDeviceId()
    {
        var deviceId = _context.HttpContext.User.FindFirstValue(DEVICE_ID_CLAIM);

        if (deviceId == null) throw new NotFoundException();

        return DeviceId.Parse(deviceId);
    }

    public DeviceId? GetDeviceIdOrNull()
    {
        var deviceId = _context.HttpContext.User.FindFirstValue(DEVICE_ID_CLAIM);

        return deviceId == null ? null : DeviceId.Parse(deviceId);
    }

    public string GetUserId()
    {
        var userId = _context.HttpContext.User.FindFirstValue(USER_ID_CLAIM);

        if (userId == null) throw new NotFoundException();

        return userId;
    }

    public string GetUserIdOrNull()
    {
        var userId = _context.HttpContext.User.FindFirstValue(USER_ID_CLAIM);

        return userId;
    }

    public IEnumerable<string> GetRoles()
    {
        var rolesClaim = _context.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        if (rolesClaim == null) return new List<string>();

        return rolesClaim.Split(' ');
    }

    public SubscriptionPlan GetSubscriptionPlan()
    {
        var subscriptionPlanClaim = _context.HttpContext.User.FindFirstValue(SUBSCRIPTION_PLAN_CLAIM);

        if (string.IsNullOrEmpty(subscriptionPlanClaim)) return SubscriptionPlan.Undefined;

        switch (subscriptionPlanClaim)
        {
            case "free": return SubscriptionPlan.Free;
            case "paid": return SubscriptionPlan.Paid;
            default:
                throw new InvalidEnumArgumentException(
                    $"The value '{subscriptionPlanClaim}' is not a supported subscription plan.");
        }
    }
}