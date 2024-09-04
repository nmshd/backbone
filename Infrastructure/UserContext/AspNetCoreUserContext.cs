using System.Security.Claims;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        var address = GetHttpContext().User.FindFirstValue(ADDRESS_CLAIM) ?? throw new NotFoundException();
        return IdentityAddress.Parse(address);
    }

    public IdentityAddress? GetAddressOrNull()
    {
        var address = GetHttpContext().User.FindFirstValue(ADDRESS_CLAIM);

        return address == null ? null : IdentityAddress.Parse(address);
    }

    public DeviceId GetDeviceId()
    {
        var deviceId = GetHttpContext().User.FindFirstValue(DEVICE_ID_CLAIM) ?? throw new NotFoundException();
        return DeviceId.Parse(deviceId);
    }

    public DeviceId? GetDeviceIdOrNull()
    {
        var deviceId = GetHttpContext().User.FindFirstValue(DEVICE_ID_CLAIM);

        return deviceId == null ? null : DeviceId.Parse(deviceId);
    }

    public string GetUserId()
    {
        var userId = GetHttpContext().User.FindFirstValue(USER_ID_CLAIM) ?? throw new NotFoundException();
        return userId;
    }

    public string? GetUserIdOrNull()
    {
        var userId = GetHttpContext().User.FindFirstValue(USER_ID_CLAIM);

        return userId;
    }

    public string GetUsername()
    {
        return GetUsernameOrNull() ?? throw new NotFoundException();
    }

    public string? GetUsernameOrNull()
    {
        var username = GetHttpContext().User.Identities.FirstOrDefault()?.Name;
        return username;
    }

    private HttpContext GetHttpContext()
    {
        return _context.HttpContext ?? throw new Exception("HttpContext is null");
    }
}
