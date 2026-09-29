using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomExceptionFilter : ExceptionFilterAttribute
{
    private const string RequestBodyTooLargeErrorCode = "error.platform.requestBodyTooLarge";
    private readonly ILogger<CustomExceptionFilter> _logger;
    private readonly HttpExceptionResponseFactory _responseFactory;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _responseFactory = new HttpExceptionResponseFactory(environment);
    }

    public override void OnException(ExceptionContext context)
    {
        LogException(context.Exception, context.HttpContext.Request.GetDisplayUrl());

        var response = _responseFactory.Create(context.Exception);
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = (int)response.StatusCode;
        context.Result = new JsonResult(HttpResponseEnvelope.CreateError(response.Error), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private void LogException(Exception exception, string uri)
    {
        switch (exception)
        {
            case InfrastructureException infrastructureException:
                _logger.InvalidUserInput(nameof(InfrastructureException), infrastructureException.Code, infrastructureException.Message);
                break;
            case ApplicationException applicationException:
                _logger.InvalidUserInput(nameof(ApplicationException), applicationException.Code, applicationException.Message);
                break;
            case DomainException domainException:
                _logger.InvalidUserInput(nameof(DomainException), domainException.Code, domainException.Message);
                break;
            case BadHttpRequestException:
                _logger.RequestBodyTooLarge(RequestBodyTooLargeErrorCode);
                break;
            default:
                _logger.ErrorWhileProcessingRequestToUri(uri, exception);
                break;
        }
    }
}

internal static partial class ExceptionFilterLogs
{
    [LoggerMessage(EventId = 799306, EventName = "ExceptionFilter.InvalidUserInput", Level = LogLevel.Information,
        Message = "An '{exception}' occurred. Error Code: '{code}'. Error message: '{message}'.")]
    public static partial void InvalidUserInput(this ILogger logger, string exception, string code, string message);

    [LoggerMessage(EventId = 938218, EventName = "ExceptionFilter.RequestBodyTooLarge", Level = LogLevel.Information,
        Message = "'{errorCode}': The body of the request is too large.")]
    public static partial void RequestBodyTooLarge(this ILogger logger, string errorCode);

    [LoggerMessage(EventId = 259125, EventName = "ExceptionFilter.ErrorWhileProcessingRequestToUri", Level = LogLevel.Error,
        Message = "Unexpected Error while processing request to '{uri}'.")]
    public static partial void ErrorWhileProcessingRequestToUri(this ILogger logger, string uri, Exception ex);
}
