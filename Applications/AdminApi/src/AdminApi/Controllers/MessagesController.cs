using Backbone.AdminApi.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class MessagesController : ApiControllerBase
{
    private readonly AdminApiDbContext _adminUiDbContext;
    private readonly ApplicationConfiguration _configuration;

    public MessagesController(IMediator mediator, IOptions<ApplicationConfiguration> options, AdminApiDbContext adminUiDbContext) : base(mediator)
    {
        _adminUiDbContext = adminUiDbContext;
        _configuration = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<List<MessageOverviewDTO>>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListMessages([FromQuery] string participant, [FromQuery] string type, [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var query = _adminUiDbContext.Messages
            .Include(m => m.Recipients)
            .Include(m => m.Attachments)
            .AsSplitQuery()
            .Select(m => new MessageOverviewDTO
            {
                MessageId = m.Id,
                SenderAddress = m.CreatedBy,
                SendDate = m.CreatedAt,
                SenderDevice = m.CreatedByDevice,
                Recipients = m.Recipients,
                NumberOfAttachments = m.Attachments.Count(),
            });

        query = type switch
        {
            "Incoming" => query.Where(m => m.Recipients.Any(r => r.Address == participant)),
            "Outgoing" => query.Where(m => m.SenderAddress == participant),
            _ => throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue(nameof(type))),
        };

        var messages = await query.OrderAndPaginate(m => m.SendDate, paginationFilter, cancellationToken);

        return Paged(new PagedResponse<MessageOverviewDTO>(messages.ItemsOnPage, paginationFilter, messages.TotalNumberOfItems));
    }
}
