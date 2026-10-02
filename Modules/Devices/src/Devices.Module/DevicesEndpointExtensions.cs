using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Devices.Module.Features.Authorization.ExchangeToken;
using Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;
using Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;
using Backbone.Modules.Devices.Module.Features.Devices.GetActiveDevice;
using Backbone.Modules.Devices.Module.Features.Devices.ListDevices;
using Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;
using Backbone.Modules.Devices.Module.Features.Devices.UpdateActiveDevice;
using Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;
using Backbone.Modules.Devices.Module.Features.Identities.ChangeFeatureFlags;
using Backbone.Modules.Devices.Module.Features.Identities.CreateIdentity;
using Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;
using Backbone.Modules.Devices.Module.Features.Identities.GetOwnIdentity;
using Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;
using Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsOwner;
using Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;
using Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;
using Backbone.Modules.Devices.Module.Features.Notifications.SendNotification;
using Backbone.Modules.Devices.Module.Features.PushNotifications.DeleteDeviceRegistration;
using Backbone.Modules.Devices.Module.Features.PushNotifications.SendTestNotification;
using Backbone.Modules.Devices.Module.Features.PushNotifications.UpdateDeviceRegistration;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Devices.Module;

public static class DevicesEndpointExtensions
{
    public static IEndpointRouteBuilder MapDevicesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var devices = endpoints.MapVersionedEndpointGroup("Devices", "Devices", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        devices.MapRegisterDeviceEndpoint();
        devices.MapUpdateActiveDeviceEndpoint();
        devices.MapChangePasswordEndpoint();
        devices.MapListDevicesEndpoint();
        devices.MapGetActiveDeviceEndpoint();
        devices.MapDeleteDeviceEndpoint();

        var identities = endpoints.MapVersionedEndpointGroup("Identities", "Identities", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        identities.MapCreateIdentityEndpoint();
        identities.MapGetOwnIdentityEndpoint();
        identities.MapIsIdentityOfUserDeletedEndpoint();
        identities.MapChangeFeatureFlagsEndpoint();
        identities.MapListFeatureFlagsEndpoint();

        var deletion = endpoints.MapVersionedEndpointGroup("Identities/Self/DeletionProcesses", "IdentityDeletionProcesses", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        deletion.MapStartDeletionProcessEndpoint();
        deletion.MapGetDeletionProcessAsOwnerEndpoint();
        deletion.MapListDeletionProcessesAsOwnerEndpoint();
        deletion.MapCancelDeletionProcessEndpoint();

        var push = endpoints.MapVersionedEndpointGroup("Devices/Self/PushNotifications", "PushNotifications", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        push.MapUpdateDeviceRegistrationEndpoint();
        push.MapDeleteDeviceRegistrationEndpoint();
        push.MapSendTestNotificationEndpoint();

        var notifications = endpoints.MapVersionedEndpointGroup("Notifications", "Notifications", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        notifications.MapSendNotificationEndpoint();

        endpoints.MapExchangeTokenEndpoint();
        return endpoints;
    }
}
