using Backbone.API;
using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Synchronization.Application;
using Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Application.Datawallets.Queries.GetDatawallet;
using Synchronization.Application.Datawallets.Queries.GetModifications;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Synchronization.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
public class DatawalletController : ApiControllerBase
{
    private readonly ApplicationOptions _options;


    public DatawalletController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<DatawalletDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetDatawalletQuery());
        return Ok(response);
    }

    [HttpGet("Modifications")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetModificationsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModifications([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] int? localIndex,
        [FromHeader(Name = "X-Supported-Datawallet-Version")] ushort supportedDatawalletVersion)
    {
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        var request = new GetModificationsQuery(paginationFilter, localIndex, supportedDatawalletVersion);

        var response = await _mediator.Send(request);
        return Paged(response);
    }

    [HttpPost("Modifications")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<PushDatawalletModificationsResponse>),
        StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PushModifications(PushDatawalletModificationsRequestBody request,
        [FromHeader(Name = "X-Supported-Datawallet-Version")] ushort supportedDatawalletVersion)
    {
        var response = await _mediator.Send(new PushDatawalletModificationsCommand(request.Modifications,
            request.LocalIndex, supportedDatawalletVersion));
        return Created(string.Empty, response);
    }
}

public class PushDatawalletModificationsRequestBody
{
    public long? LocalIndex { get; set; }
    public PushDatawalletModificationItem[] Modifications { get; set; }
}