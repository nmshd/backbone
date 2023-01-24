using Devices.Application.Devices.DTOs;
using MediatR;

namespace Devices.Application.Devices.Queries.GetActiveDevice;

public class GetActiveDeviceQuery : IRequest<DeviceDTO> { }
