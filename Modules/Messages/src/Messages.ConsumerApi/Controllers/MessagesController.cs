using Backbone.Modules.Messages.Application;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;
using Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Messages.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class MessagesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public MessagesController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListMessagesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMessages([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<MessageId> ids, CancellationToken cancellationToken)
    {
        var command = new ListMessagesQuery(paginationFilter, ids);

        command.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (command.PaginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var messages = await _mediator.Send(command, cancellationToken);
        return Paged(messages);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<MessageDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage(MessageId id, [FromQuery] bool? noBody, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetMessageQuery { Id = id, NoBody = noBody == true }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<SendMessageResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetMessage), new { id = response.Id }, response);
    }
}
