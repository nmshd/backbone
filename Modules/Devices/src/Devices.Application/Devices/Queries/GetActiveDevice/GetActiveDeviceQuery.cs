using Backbone.Modules.Devices.Application.Devices.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;

public class GetActiveDeviceQuery : IRequest<DeviceDTO>;
