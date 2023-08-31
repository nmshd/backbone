﻿using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationCommand : IRequest<Unit>
{
    public string Platform { get; set; }
    public string Handle { get; set; }
    public string AppId { get; set; }
}
