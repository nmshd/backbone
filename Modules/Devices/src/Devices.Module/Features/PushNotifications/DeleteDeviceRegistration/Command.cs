using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeleteDeviceRegistration;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteDeviceRegistrationCommand")]
public class Command : IRequest;
