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
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceId()
    {
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceIdOrNull()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetRoles()
    {
        throw new NotImplementedException();
    }

    public SubscriptionPlan GetSubscriptionPlan()
    {
        throw new NotImplementedException();
    }

    public string GetUserId()
    {
        throw new NotImplementedException();
    }

    public string GetUserIdOrNull()
    {
        throw new NotImplementedException();
    }

    public string GetUsername()
    {
        throw new NotImplementedException();
    }

    public string GetUsernameOrNull()
    {
        throw new NotImplementedException();
    }
}
