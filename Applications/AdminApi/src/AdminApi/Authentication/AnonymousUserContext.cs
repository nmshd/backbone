using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.AdminApi.Authentication;

public class AnonymousUserContext : IUserContext
{
    public IdentityAddress GetAddress()
    {
        throw new NotSupportedException();
    }

    public IdentityAddress? GetAddressOrNull()
    {
        return null;
    }

    public DeviceId GetDeviceId()
    {
        throw new NotSupportedException();
    }

    public DeviceId? GetDeviceIdOrNull()
    {
        return null;
    }

    public string GetUserId()
    {
        throw new NotSupportedException();
    }

    public string GetUserIdOrNull()
    {
        throw new NotSupportedException();
    }

    public string GetUsername()
    {
        throw new NotSupportedException();
    }

    public string GetUsernameOrNull()
    {
        throw new NotSupportedException();
    }

    public string GetClientId()
    {
        throw new NotSupportedException();
    }

    public string? GetClientIdOrNull()
    {
        return null;
    }
}
