﻿using Backbone.Modules.Devices.Application;
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
    private readonly ILoggerFactory _loggerFactory;

    public LogsController(
        IMediator mediator, IOptions<ApplicationOptions> options, ILoggerFactory logger) : base(mediator)
    {
        _options = options.Value;
        _loggerFactory = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public IActionResult CreateLog(LogRequest request)
    {
        var logger = _loggerFactory.CreateLogger(request.Category);

        switch (request.LogLevel)
        {
            case LogLevel.Trace:
                logger.LogTrace(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.Debug:
                logger.LogDebug(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.Information:
            case LogLevel.Log:
                logger.LogInformation(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.Warning:
                logger.LogWarning(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.Error:
                logger.LogError(request.MessageTemplate, request.Arguments);
                break;
            case LogLevel.Critical:
                logger.LogCritical(request.MessageTemplate, request.Arguments);
                break;
            default:
                throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue(nameof(request.LogLevel)));
                break;
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
    Trace,
    Debug,
    Information,
    Log,
    Warning,
    Error,
    Critical
}
