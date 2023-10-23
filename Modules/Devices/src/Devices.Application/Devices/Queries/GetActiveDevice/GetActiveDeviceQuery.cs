using Backbone.Devices.Application.Devices.DTOs;
using MediatR;

namespace Backbone.Devices.Application.Devices.Queries.GetActiveDevice;

public class GetActiveDeviceQuery : IRequest<DeviceDTO> { }
