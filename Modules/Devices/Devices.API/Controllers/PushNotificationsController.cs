using Devices.Application.PushNotifications.Commands.SendTestNotification;
using Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API.Controllers;

[Route("api/v1/Devices/Self/[controller]")]
[Authorize(IdentityServerConstants.LocalApi.PolicyName)]
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
        await _mediator.Send(new SendTestNotificationCommand {Data = data});
        return NoContent();
    }
}
