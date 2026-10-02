using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.GetActiveDevice;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetActiveDeviceQuery")]
public class Query : IRequest<DeviceDTO>;
