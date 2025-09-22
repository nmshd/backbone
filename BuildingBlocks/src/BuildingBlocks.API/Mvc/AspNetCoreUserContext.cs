using System.Security.Claims;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Mvc;

public class AspNetCoreUserContext : IUserContext
{
    private const string ADDRESS_CLAIM_OLD = "address";
    private const string ADDRESS_CLAIM = "nmshd_address";
    private const string DEVICE_ID_CLAIM = "device_id";
    private const string USER_ID_CLAIM = "sub";
    private const string CLIENT_ID_CLAIM = "client_id";
    private readonly IHttpContextAccessor _context;

    public AspNetCoreUserContext(IHttpContextAccessor context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IdentityAddress GetAddress()
    {
        var address = GetHttpContext().User.FindFirstValue(ADDRESS_CLAIM) ?? GetHttpContext().User.FindFirstValue(ADDRESS_CLAIM_OLD) ?? throw new NotFoundException();
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

    public string GetClientId()
    {
        return GetClientIdOrNull() ?? throw new NotFoundException();
    }

    public string? GetClientIdOrNull()
    {
        var clientId = GetHttpContext().User.FindFirstValue(CLIENT_ID_CLAIM);
        return clientId;
    }

    private HttpContext GetHttpContext()
    {
        return _context.HttpContext ?? throw new Exception("HttpContext is null");
    }
}
