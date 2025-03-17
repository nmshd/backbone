using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
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
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
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
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<List<MessageOverview>>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMessages([FromQuery] string participant, [FromQuery] string type, [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        switch (type)
        {
            case "Incoming":
                var incomingMessages = await _adminUiDbContext.MessageOverviews
                    .Where(m => m.Recipients.Any(r => r.Address == participant))
                    .IncludeAll(_adminUiDbContext)
                    .OrderAndPaginate(d => d.SendDate, paginationFilter, cancellationToken);

                return Paged(new PagedResponse<MessageOverview>(incomingMessages.ItemsOnPage, paginationFilter, incomingMessages.TotalNumberOfItems));

            case "Outgoing":
                var outgoingMessages = await _adminUiDbContext.MessageOverviews
                    .Where(m => m.SenderAddress == participant)
                    .IncludeAll(_adminUiDbContext)
                    .OrderAndPaginate(d => d.SendDate, paginationFilter, cancellationToken);

                return Paged(new PagedResponse<MessageOverview>(outgoingMessages.ItemsOnPage, paginationFilter, outgoingMessages.TotalNumberOfItems));
        }

        throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue(nameof(type)));
    }
}
