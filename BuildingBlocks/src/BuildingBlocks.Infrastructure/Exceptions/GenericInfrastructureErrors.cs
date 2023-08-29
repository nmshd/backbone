namespace Enmeshed.BuildingBlocks.Infrastructure.Exceptions;

public static class GenericInfrastructureErrors
{
    public static InfrastructureError InvalidPushNotificationConfiguration()
    {
        return new InfrastructureError("error.platform.invalidConfiguration",
            "Invalid or non existent configuration for push notifications application id.");
    }
}
