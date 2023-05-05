using Backbone.Modules.Devices.Application.PushNotifications.Commands.SendTestNotification;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Devices.ConsumerApi.Controllers;

[Route("api/v1/Devices/Self/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class PushNotificationsController : ApiControllerBase
{
    public PushNotificationsController(IMediator mediator) : base(mediator) { }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterForPushNotifications(UpdateDeviceRegistrationCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpPost("SendTestNotification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendTestPushNotification([FromBody] dynamic data)
    {
        await _mediator.Send(new SendTestNotificationCommand { Data = data });
        return NoContent();
    }
}
