using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.UnitTestTools.TestDoubles;

public class UserContextStub : IUserContext
{
    private readonly IdentityAddress? _identityAddress;
    private readonly DeviceId? _deviceId;

    private UserContextStub(IdentityAddress? identityAddress, DeviceId? deviceId)
    {
        _identityAddress = identityAddress;
        _deviceId = deviceId;
    }

    public static UserContextStub ForAuthenticatedUser()
    {
        return new UserContextStub(null, null);
    }

    public static UserContextStub ForUnauthenticatedUser()
    {
        return new UserContextStub(IdentityAddress.Create([0], "prod.enmeshed.eu"), DeviceId.New());
    }

    public IdentityAddress GetAddress()
    {
        return _identityAddress ?? throw new NotFoundException();
    }

    public IdentityAddress? GetAddressOrNull()
    {
        return _identityAddress;
    }

    public DeviceId GetDeviceId()
    {
        return _deviceId ?? throw new NotFoundException();
    }

    public DeviceId? GetDeviceIdOrNull()
    {
        return _deviceId;
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
        throw new NotSupportedException();
    }
}
