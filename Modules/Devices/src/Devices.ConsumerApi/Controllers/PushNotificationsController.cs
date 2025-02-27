using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.SendTestNotification;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Devices.ConsumerApi.Controllers;

[Route("api/v1/Devices/Self/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class PushNotificationsController : ApiControllerBase
{
    public PushNotificationsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPut]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<UpdateDeviceRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterForPushNotifications(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnregisterFromPushNotifications(CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDeviceRegistrationCommand(), cancellationToken);
        return NoContent();
    }

    [HttpPost("SendTestNotification")]
    [ApiExplorerSettings(IgnoreApi = true)] // don't show this in the API docs as it's just for internal testing 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendTestPushNotification([FromBody] dynamic data, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendTestNotificationCommand { Data = data }, cancellationToken);
        return NoContent();
    }
}
