using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

internal class UserContextStub : IUserContext
{
    public IdentityAddress GetAddress()
    {
        return IdentityAddress.Create([0], "id1");
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

    public IEnumerable<string> GetRoles()
    {
        throw new NotSupportedException();
    }

    public SubscriptionPlan GetSubscriptionPlan()
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
}
