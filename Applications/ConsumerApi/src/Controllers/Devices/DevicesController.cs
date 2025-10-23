using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;
using Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Devices;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class DevicesController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public DevicesController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RegisterDeviceResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice(RegisterDeviceRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterDeviceCommand
        {
            CommunicationLanguage = request.CommunicationLanguage ?? CommunicationLanguage.DEFAULT_LANGUAGE.Value,
            SignedChallenge = request.SignedChallenge,
            DevicePassword = request.DevicePassword,
            IsBackupDevice = request.IsBackupDevice ?? false
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created("", response);
    }

    [HttpPut("Self")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateActiveDevice(UpdateActiveDeviceCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);

        return NoContent();
    }

    [HttpPut("Self/Password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListDevicesResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListDevices([FromQuery] PaginationFilter paginationFilter, [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var response = await _mediator.Send(new ListDevicesQuery { PaginationFilter = paginationFilter, Ids = ids }, cancellationToken);

        return Paged(response);
    }

    [HttpGet("Self")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<DeviceDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveDevice()
    {
        var response = await _mediator.Send(new GetActiveDeviceQuery());
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice([FromRoute] string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDeviceCommand { DeviceId = id }, cancellationToken);
        return NoContent();
    }
}

public class RegisterDeviceRequest
{
    public required string DevicePassword { get; set; }
    public string? CommunicationLanguage { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
    public bool? IsBackupDevice { get; set; }
}
