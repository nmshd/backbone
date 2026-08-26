using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.EventHandlerService;

public class AnonymousUserContext : IUserContext
{
    public IdentityAddress GetAddress()
    {
        throw new NotFoundException();
    }

    public IdentityAddress? GetAddressOrNull()
    {
        return null;
    }

    public DeviceId GetDeviceId()
    {
        throw new NotFoundException();
    }

    public DeviceId? GetDeviceIdOrNull()
    {
        return null;
    }

    public string GetUserId()
    {
        throw new NotFoundException();
    }

    public string? GetUserIdOrNull()
    {
        return null;
    }

    public string GetUsername()
    {
        throw new NotFoundException();
    }

    public string? GetUsernameOrNull()
    {
        return null;
    }

    public string GetClientId()
    {
        throw new NotFoundException();
    }

    public string? GetClientIdOrNull()
    {
        return null;
    }
}
