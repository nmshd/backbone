﻿using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationCommand : IRequest<Unit>, IMapTo<DeviceRegistration>
{
    public string Platform { get; set; }
    public string Handle { get; set; }
    public string AppId { get; set; }
}
