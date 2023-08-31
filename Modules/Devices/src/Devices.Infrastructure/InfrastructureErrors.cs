using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;

namespace Backbone.Modules.Devices.Infrastructure;
public static class InfrastructureErrors
{
    public static InfrastructureError InvalidPushNotificationConfiguration(List<string> supportedAppIds)
    {
        var supportedAppIdsString = string.Join(", ", supportedAppIds.Select(x => $"'{x}'"));

        return new InfrastructureError("error.platform.unsupportedAppId",
            $"The given app id is not supported. Supported app ids are: {supportedAppIdsString}.");
    }
}
