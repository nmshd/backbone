using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

internal class UserContextStub : IUserContext
{
    public IdentityAddress GetAddress()
    {
        return IdentityAddress.Create([0], "prod.enmeshed.eu");
    }

    public IdentityAddress GetAddressOrNull()
    {
        throw new NotSupportedException();
    }

    public DeviceId GetDeviceId()
    {
        throw new NotSupportedException();
    }

    public DeviceId GetDeviceIdOrNull()
    {
        throw new NotSupportedException();
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
