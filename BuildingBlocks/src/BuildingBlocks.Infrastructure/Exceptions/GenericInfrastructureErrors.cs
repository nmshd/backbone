namespace Enmeshed.BuildingBlocks.Infrastructure.Exceptions;

public static class GenericInfrastructureErrors
{
    public static InfrastructureError InvalidPushNotificationConfiguration(List<string> supportedAppIds)
    {
        return new InfrastructureError("error.platform.unsupportedAppId",
            $"The given app id is not supported. Supported app ids are: {string.Join(", ", supportedAppIds)}.");
    }
}
