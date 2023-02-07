using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;
using Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class DevicesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public DevicesController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RegisterDeviceResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice(RegisterDeviceCommand request)
    {
        var response = await _mediator.Send(request);

        return Created("", response);
    }

    [HttpPut("Self/Password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand request)
    {
        await _mediator.Send(request);

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListDevicesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDevices([FromQuery] PaginationFilter paginationFilter, [FromQuery] IEnumerable<DeviceId> ids)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new ListDevicesQuery(paginationFilter, ids));

        return Paged(response);
    }

    [HttpGet("Self")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<DeviceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveDevice()
    {
        var response = await _mediator.Send(new GetActiveDeviceQuery());
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice([FromRoute] DeviceId id, [FromBody] DeleteDeviceRequest request)
    {
        await _mediator.Send(new DeleteDeviceCommand { DeviceId = id, DeletionCertificate = request.DeletionCertificate, SignedChallenge = request.SignedChallenge });

        return NoContent();
    }
}

public class DeleteDeviceRequest
{
    public byte[] DeletionCertificate { get; set; }
    public SignedChallengeDTO SignedChallenge { get; set; }
}
