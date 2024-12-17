using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomExceptionFilter : ExceptionFilterAttribute
{
    private const string ERROR_CODE_UNEXPECTED_EXCEPTION = "error.platform.unexpected";
    private const string ERROR_CODE_REQUEST_BODY_TOO_LARGE = "error.platform.requestBodyTooLarge";

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public override void OnException(ExceptionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";

        HttpError httpError;

        switch (context.Exception)
        {
            case InfrastructureException infrastructureException:
                _logger.InvalidUserInput(nameof(InfrastructureException), infrastructureException.Code, infrastructureException.Message);

                httpError = CreateHttpErrorForInfrastructureException(infrastructureException);

                context.HttpContext.Response.StatusCode =
                    (int)GetStatusCodeForInfrastructureException(infrastructureException);

                break;
            case ApplicationException applicationException:
                _logger.InvalidUserInput(nameof(ApplicationException), applicationException.Code, applicationException.Message);

                httpError = CreateHttpErrorForApplicationException(applicationException);

                context.HttpContext.Response.StatusCode =
                    (int)GetStatusCodeForApplicationException(applicationException);

                break;

            case DomainException domainException:
                _logger.InvalidUserInput(nameof(DomainException), domainException.Code, domainException.Message);

                httpError = CreateHttpErrorForDomainException(domainException);

                context.HttpContext.Response.StatusCode = (int)GetStatusCodeForDomainException(domainException);

                break;
            case BadHttpRequestException:
                _logger.RequestBodyTooLarge(ERROR_CODE_REQUEST_BODY_TOO_LARGE);

                httpError = HttpError.ForProduction(
                    ERROR_CODE_REQUEST_BODY_TOO_LARGE,
                    "The request body is too large.",
                    "" // TODO: add documentation link
                );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                break;
            default:
                _logger.ErrorWhileProcessingRequestToUri(context.HttpContext.Request.GetDisplayUrl(), context.Exception);

                httpError = CreateHttpErrorForUnexpectedException(context);

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                break;
        }

        context.Result = new JsonResult(HttpResponseEnvelope.CreateError(httpError),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            });
    }

    private static HttpError CreateHttpErrorForInfrastructureException(InfrastructureException infrastructureException)
    {
        var httpError = HttpError.ForProduction(
            infrastructureException.Code,
            infrastructureException.Message,
            "" // TODO: add documentation link
        );

        return httpError;
    }

    private HttpError CreateHttpErrorForApplicationException(ApplicationException applicationException)
    {
        var httpError = HttpError.ForProduction(
            applicationException.Code,
            applicationException.Message,
            "", // TODO: add documentation link
            data: GetCustomData(applicationException)
        );

        return httpError;
    }

    private static HttpError CreateHttpErrorForDomainException(DomainException domainException)
    {
        var httpError = HttpError.ForProduction(
            domainException.Code,
            domainException.Message,
            "" // TODO: add documentation link
        );

        return httpError;
    }

    private static dynamic? GetCustomData(ApplicationException applicationException)
    {
        if (applicationException is QuotaExhaustedException quotaExhaustedException)
        {
            return quotaExhaustedException.ExhaustedMetricStatuses.Select(m => new
            {
#pragma warning disable IDE0037
                MetricKey = m.MetricKey.Value,
                IsExhaustedUntil = m.IsExhaustedUntil
#pragma warning restore IDE0037
            });
        }

        return applicationException.AdditionalData;
    }

    private static HttpStatusCode GetStatusCodeForInfrastructureException(InfrastructureException _)
    {
        return HttpStatusCode.BadRequest;
    }

    private static HttpStatusCode GetStatusCodeForApplicationException(ApplicationException exception)
    {
        return exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ActionForbiddenException => HttpStatusCode.Forbidden,
            QuotaExhaustedException => HttpStatusCode.TooManyRequests,
            _ => HttpStatusCode.BadRequest
        };
    }

    private static HttpStatusCode GetStatusCodeForDomainException(DomainException exception)
    {
        if (exception.Code == GenericDomainErrors.NotFound().Code)
            return HttpStatusCode.NotFound;
        if (exception.Code == GenericDomainErrors.Forbidden().Code)
            return HttpStatusCode.Forbidden;

        return HttpStatusCode.BadRequest;
    }

    private HttpError CreateHttpErrorForUnexpectedException(ExceptionContext context)
    {
        HttpError httpError;

        if (_env.IsDevelopment() || _env.IsLocal())
        {
            var details = context.Exception.Message;
            var innerException = context.Exception.InnerException;

            while (innerException != null)
            {
                details += "\r\n> " + innerException.Message;
                innerException = innerException.InnerException;
            }

            httpError = HttpError.ForDev(
                ERROR_CODE_UNEXPECTED_EXCEPTION,
                "An unexpected error occurred.",
                "", // TODO: add documentation link
                GetFormattedStackTrace(context.Exception),
                details
            );
        }
        else
        {
            httpError = HttpError.ForProduction(
                ERROR_CODE_UNEXPECTED_EXCEPTION,
                "An unexpected error occurred.",
                "" // TODO: add documentation link
            );
        }

        return httpError;
    }

    private static IEnumerable<string> GetFormattedStackTrace(Exception exception)
    {
        if (exception.StackTrace == null)
            return [];

        return
            Regex.Matches(exception.StackTrace, "at .+").Select(m => m.Value.Trim());
    }
}

internal static partial class ExceptionFilterLogs
{
    [LoggerMessage(
        EventId = 799306,
        EventName = "ExceptionFilter.InvalidUserInput",
        Level = LogLevel.Information,
        Message = "An '{exception}' occurred. Error Code: '{code}'. Error message: '{message}'.")]
    public static partial void InvalidUserInput(this ILogger logger, string exception, string code, string message);

    [LoggerMessage(
        EventId = 938218,
        EventName = "ExceptionFilter.RequestBodyTooLarge",
        Level = LogLevel.Information,
        Message = "'{errorCode}': The body of the request is too large.")]
    public static partial void RequestBodyTooLarge(this ILogger logger, string errorCode);

    [LoggerMessage(
        EventId = 259125,
        EventName = "ExceptionFilter.ErrorWhileProcessingRequestToUri",
        Level = LogLevel.Error,
        Message = "Unexpected Error while processing request to '{uri}'.")]
    public static partial void ErrorWhileProcessingRequestToUri(this ILogger logger, string uri, Exception ex);
}
