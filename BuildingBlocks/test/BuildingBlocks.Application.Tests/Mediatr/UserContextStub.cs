using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

public class UserContextStub : IUserContext
{
    public IdentityAddress GetAddress()
    {
        return IdentityAddress.Create(Convert.FromBase64String("mJGmNbxiVZAPToRuk9O3NvdfsWl6V+7wzIc+/57bU08="), "id1");
    }

    public IdentityAddress GetAddressOrNull()
    {
        return GetAddress();
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
}