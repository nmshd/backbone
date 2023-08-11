using Backbone.Modules.Devices.Application;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class LogsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;
    private readonly ILoggerFactory _logger;

    public LogsController(
        IMediator mediator, IOptions<ApplicationOptions> options, ILoggerFactory logger) : base(mediator)
    {
        _options = options.Value;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public IActionResult PostTiers([FromBody] LogRequest request)
    {
        var logger = _logger.CreateLogger(request.Category);

        switch (request.LogLevel)
        {
            case LogLevel.TRACE:
                logger.LogTrace(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.DEBUG:
                logger.LogDebug(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.INFORMATION: case LogLevel.LOG:
                logger.LogInformation(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.WARNING:
                logger.LogWarning(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.ERROR:
                logger.LogError(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.CRITICAL:
                logger.LogCritical(request.MessageTemplate, request.Arguments);
                break;
            default:
                throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(request.LogLevel)));
        }

        return NoContent();
    }
}

public class LogRequest
{
    public LogLevel LogLevel { get; set; }
    public string Category { get; set; }
    public string MessageTemplate { get; set; }
    public object[] Arguments { get; set; }
}

public enum LogLevel
{
    TRACE,
    DEBUG,
    INFORMATION,
    LOG,
    WARNING,
    ERROR,
    CRITICAL
}
